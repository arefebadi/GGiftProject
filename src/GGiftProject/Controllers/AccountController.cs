using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GGiftProject.Models;
using GGiftProject.Services;
using GGiftProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace GGiftProject.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<IdentityUser> _userManagerService;
        private SignInManager<IdentityUser> _signinManagerService;
        private RoleManager<IdentityRole> _roleManagerService;
        private IDataService<Profile> _profileDataService;
        private IDataService<Address> _AddressDataService;
        public AccountController(UserManager<IdentityUser> managerService, SignInManager<IdentityUser> singInService, RoleManager<IdentityRole> roleService, IDataService<Profile> profileDataService, IDataService<Address> addressService
            )
        {
            _userManagerService = managerService;
            _signinManagerService = singInService;
            _roleManagerService = roleService;
            _profileDataService = profileDataService;
            _AddressDataService = addressService;

        }



        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(AccountRegisterViewModel vm)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = new IdentityUser(vm.Email);
                user.Email = vm.Email;
                IdentityResult result = await _userManagerService.CreateAsync(user, vm.Password);
                if (result.Succeeded)
                {
                    await _userManagerService.AddToRoleAsync(user, "Customer");

                    //add a new profile
                    Profile newProfile = new Profile { Email = vm.Email, FirstName = vm.FirstName, LastName = vm.LastName, DOB = vm.DOB, IsAdmin = false };
                    _profileDataService.Create(newProfile);

                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(vm);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "")
        {
            AccountLoginViewModel vm = new AccountLoginViewModel
            {
                ReturnUrl = returnUrl
            };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AccountLoginViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var results = await _signinManagerService.PasswordSignInAsync(vm.Email, vm.Password, vm.RememberMe, false);
                if (results.Succeeded)
                {
                    ShoppingCartService.SetCartId(HttpContext);
                    if (!string.IsNullOrEmpty(vm.ReturnUrl))
                    {
                        return Redirect(vm.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ModelState.AddModelError("", "Wrong Input in Email or Password");
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signinManagerService.SignOutAsync();
            ShoppingCartService.ResetCartId(HttpContext);
            return RedirectToAction("Index", "Home");
        }



        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AddRole()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddRole(AccountAddRoleViewModel vm)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole(vm.Name);
                IdentityResult result = await _roleManagerService.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    //show errors
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(vm);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UpdateProfile()
        {
            IdentityUser user = await _userManagerService.FindByNameAsync(User.Identity.Name);


            Profile profile = _profileDataService.GetSingle(p => p.Email == user.Email);


            AccountUpdateProfileViewModel vm;

            if (profile != null)

            {
                IEnumerable<Address> list = _AddressDataService.Query(x => x.ProfileId == profile.ProfileId);

                vm = new AccountUpdateProfileViewModel
                {


                    ProfileId = profile.ProfileId,
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    //IsAdmin = profile.IsAdmin,
                    DOB = profile.DOB,
                    Email = profile.Email,
                    Addresses = list.Count() == 0 ?
                    new List<AccountAddressViewModel> { new AccountAddressViewModel() } :
                    list.Select(x => new AccountAddressViewModel()
                    {
                        AddressId = x.AddressId,
                        Address1 = x.Address1,
                        Address2 = x.Address2,
                        PostCode = x.PostCode,
                        Suburb = x.Suburb,
                        State = x.State,
                        IsFavourite = x.IsFavourite
                    }).ToList()

                };

            }
            else
            {
                vm = new AccountUpdateProfileViewModel();
            }
            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public IActionResult UpdateProfile(AccountUpdateProfileViewModel vm, string savechanges, string addaddress, string deleteAddress)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(savechanges))
                {
                    //map
                    Profile p = new Profile

                    {
                        ProfileId = vm.ProfileId,
                        FirstName = vm.FirstName,
                        LastName = vm.LastName,
                        DOB = vm.DOB,
                        //IsAdmin = vm.IsAdmin,
                        Email = User.Identity.Name,
                    };

                    //call service
                    _profileDataService.Update(p);
                    foreach (var address in vm.Addresses)
                    {
                        Address a = new Address
                        {
                            ProfileId = p.ProfileId,
                            AddressId = address.AddressId,
                            Address1 = address.Address1,
                            Suburb = address.Suburb,
                            State = address.State,
                            PostCode = address.PostCode,
                            IsFavourite = address.IsFavourite
                        };

                        if (address.AddressId != 0)
                        {
                            _AddressDataService.Update(a);
                        }
                        else
                        {
                            _AddressDataService.Create(a);
                        }
                    }

                    ////update Password
                    //IdentityUser user = await _userManagerService.FindByNameAsync(User.Identity.Name);
                    //_userManagerService.ChangePasswordAsync(user, "", "");
                    //await _userManagerService.UpdateAsync(user);
                    return RedirectToAction("Index", "Home");
                }
                else if (!string.IsNullOrEmpty(addaddress))
                {
                    vm.Addresses.Add(new AccountAddressViewModel());
                }
                else if (!string.IsNullOrEmpty(deleteAddress))
                {
                    int addressId = int.Parse(deleteAddress);
                    Address address = _AddressDataService.GetSingle(a => a.AddressId == addressId);
                    if (address != null)
                    {
                        _AddressDataService.Remove(address);
                    }

                    IEnumerable<Address> list = _AddressDataService.Query(x => x.ProfileId == vm.ProfileId);

                    vm.Addresses = list.Count() == 0 ?
                        new List<AccountAddressViewModel> { new AccountAddressViewModel() } :
                        list.Select(x => new AccountAddressViewModel()
                        {
                            AddressId = x.AddressId,
                            Address1 = x.Address1,
                            Address2 = x.Address2,
                            PostCode = x.PostCode,
                            Suburb = x.Suburb,
                            State = x.State,
                            IsFavourite = x.IsFavourite
                        }).ToList();


                }

            }
            return View(vm);
        }

    }
}
