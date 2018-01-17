using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GGiftProject.Models;
using GGiftProject.Services;
using GGiftProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Sakura.AspNetCore;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace GGiftProject.Controllers
{
    public class HamperController : Controller
    {
        private IDataService<Hamper> _hamperDataService;
        private IDataService<Category> _categoryDataService;
        private IDataService<Order> _orderDataService;
        private IDataService<OrderLine> _orderLineDataService;
        private IDataService<Profile> _profileDataService;
        private IDataService<Address> _addressDataService;
        private IHostingEnvironment _environmet;


        public HamperController(IDataService<Hamper> hamperDataService, IDataService<Category> categoryDataService,
            IDataService<Order> orderDataService, IDataService<OrderLine> orderLineDataService, IDataService<Profile> profileDataService,
            IDataService<Address> addressDataService, IHostingEnvironment environment)
        {
            _hamperDataService = hamperDataService;
            _categoryDataService = categoryDataService;
            _orderDataService = orderDataService;
            _orderLineDataService = orderLineDataService;
            _profileDataService = profileDataService;
            _addressDataService = addressDataService;
            _environmet = environment;
        }

        [HttpGet]
        public IActionResult HampersList(int? page)
        {
            HamperHampersListViewModel vm = new ViewModels.HamperHampersListViewModel();
            IEnumerable<Category> categoryList = _categoryDataService.GetAll();
            vm.Categories = categoryList.Select(c => new HamperCategoryViewModel
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName
            }).ToList();
            
            List<HamperViewModel> list = _hamperDataService.Query(h => !h.Discontinue)
                .Select(h => new HamperViewModel
                {
                    HamperId = h.HamperId,
                    CategoryId = h.CategoryId,
                    Image = h.Image,
                    Name = h.Name,
                    Price = h.Price,
                    Details = h.Details,
                    Quantity = 1

                }).ToList();
            vm.Hampers = list;
            return View(vm);
        }



        [HttpPost]
        public IActionResult HampersList(HamperHampersListViewModel vm, string hampersList, string hamperIdString)
        {
            if (ModelState.IsValid)
            {
                IEnumerable<Category> categoryList = _categoryDataService.GetAll();
                vm.Categories = categoryList.Select(c => new HamperCategoryViewModel
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName
                }).ToList();

                if (!string.IsNullOrEmpty(hampersList))
                {

                    List<HamperViewModel> list = _hamperDataService
                         .Query(h => !h.Discontinue && h.CategoryId == (vm.CategoryId != 0 ? vm.CategoryId : h.CategoryId) && h.Price >= (vm.MinPrice ?? h.Price) && h.Price <= (vm.MaxPrice ?? h.Price)
                         && h.Name.Contains(string.IsNullOrEmpty(vm.Name) ? h.Name : vm.Name))
                        .Select(h => new HamperViewModel
                        {
                            HamperId = h.HamperId,
                            CategoryId = h.CategoryId,
                            Image = h.Image,
                            Name = h.Name,
                            Price = h.Price,
                            Details = h.Details,
                            Quantity = 1

                        }).ToList();

                    vm.Hampers = list;


                }
                else if (!string.IsNullOrEmpty(hamperIdString))
                {
                    int hamperId = int.Parse(hamperIdString);
                    string cartId = ShoppingCartService.GetCartId(HttpContext);

                    Order order = _orderDataService.GetSingle(o => o.CartId == cartId && !o.CheckedOut);
                    if (order == null)
                    {
                        order = new Order
                        {
                            CartId = cartId,
                            CheckedOut = false,
                            DateCreated = DateTime.Now,
                            Email = !string.IsNullOrWhiteSpace(User.Identity.Name) ? User.Identity.Name : ""
                        };
                        _orderDataService.Create(order);
                    }

                    OrderLine orderLine = _orderLineDataService.GetSingle(ol => ol.OrderId == order.OrderId && ol.HamperId == hamperId);
                    if (orderLine == null)
                    {
                        orderLine = new OrderLine
                        {
                            OrderId = order.OrderId,
                            HamperId = hamperId,
                            Quantity = 0,
                            Price = 0
                        };
                        _orderLineDataService.Create(orderLine);
                    }
                    foreach (var hamper in vm.Hampers)
                    {
                        if (hamper.HamperId == hamperId)
                        {
                            orderLine.Quantity += hamper.Quantity;
                            orderLine.Price += (hamper.Quantity * hamper.Price);
                            hamper.Quantity = 1;
                        }
                    }
                    _orderLineDataService.Update(orderLine);
                }
            }
            return View(vm);
        }

        [HttpGet]
        [Authorize]
        public IActionResult AddHamper(string hamperId)
        {
            HamperAddViewModel vm = new HamperAddViewModel();


            IEnumerable<Category> categoryList = _categoryDataService.GetAll();
            vm.Categories = categoryList.Select(c => new HamperCategoryViewModel
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName
            }).ToList();

            int hId;
            if (int.TryParse(hamperId, out hId))
            {
                Hamper hamper = _hamperDataService.GetSingle(h => h.HamperId == hId);
                if (hamper != null)
                {
                    vm.HamperId = hamper.HamperId;
                    vm.CategoryId = hamper.CategoryId;
                    vm.Image = hamper.Image;
                    vm.Name = hamper.Name;
                    vm.Price = hamper.Price;
                    vm.Details = hamper.Details;
                    vm.Discontinue = hamper.Discontinue;
                }
            }

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddHamper(HamperAddViewModel vm, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                Hamper hamper = new Hamper
                {
                    HamperId = vm.HamperId,
                    CategoryId = vm.CategoryId,
                    Name = vm.Name,
                    Price = vm.Price,
                    Details = vm.Details,
                    Discontinue = vm.Discontinue,
                    Image = vm.Image,
                    Rate = 0
                };

                if (hamper.HamperId == 0)
                {
                    _hamperDataService.Create(hamper);
                }

                if (file != null)
                {
                    var uploadPath = Path.Combine(_environmet.WebRootPath, "uploads");

                    string extention = Path.GetExtension(file.FileName);
                    string fileName = hamper.HamperId + extention;

                    using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    hamper.Image = fileName;
                }



                _hamperDataService.Update(hamper);


            }
            IEnumerable<Category> categoryList = _categoryDataService.GetAll();
            vm.Categories = categoryList.Select(c => new HamperCategoryViewModel
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName
            }).ToList();
            return View(vm);
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            HamperCheckoutViewModel vm = new ViewModels.HamperCheckoutViewModel();
            string cartId = ShoppingCartService.GetCartId(HttpContext);

            Order order = _orderDataService.GetSingle(o => o.CartId == cartId && !o.CheckedOut);
            if (order != null)
            {
                vm.OrderId = order.OrderId;
                vm.CartId = order.CartId;
                vm.Email = order.Email;
                vm.DateCreated = order.DateCreated;
                vm.CheckedOut = order.CheckedOut;

                var orderLines = _orderLineDataService.Query(ol => ol.OrderId == order.OrderId);
                if (orderLines != null && orderLines.Any())
                {
                    vm.OrderLines = orderLines.Select(ol => new HamperOrderLineViewModel
                    {
                        HamperId = ol.HamperId,
                        HamperName = _hamperDataService.GetSingle(h => h.HamperId == ol.HamperId).Name,
                        Image = _hamperDataService.GetSingle(h => h.HamperId == ol.HamperId).Image,
                        OrderId = ol.OrderId,
                        OrderLineId = ol.OrderLineId,
                        Price = ol.Price,
                        Quantity = ol.Quantity
                    }).ToList();

                    vm.TotalPrice = vm.OrderLines.Sum(ol => ol.Price);
                }
            }
            else
            {
                vm.OrderLines = new List<HamperOrderLineViewModel>();
            }

            return View(vm);
        }

        [HttpPost]
        public IActionResult Checkout(HamperCheckoutViewModel vm, string deleteOrderLine)
        {
            if (ModelState.IsValid)
            {
                if(!string.IsNullOrEmpty(deleteOrderLine))
                {
                    int orderLineId = int.Parse(deleteOrderLine);
                    var orderLine = _orderLineDataService.GetSingle(ol => ol.OrderLineId == orderLineId);

                    if (orderLine != null)
                    {
                        _orderLineDataService.Remove(orderLine);
                        var orderLines = _orderLineDataService.Query(ol => ol.OrderId == vm.OrderId);
                        if (orderLines != null && orderLines.Any())
                        {
                            vm.OrderLines = orderLines.Select(ol => new HamperOrderLineViewModel
                            {
                                HamperId = ol.HamperId,
                                HamperName = _hamperDataService.GetSingle(h => h.HamperId == ol.HamperId).Name,
                                Image = _hamperDataService.GetSingle(h => h.HamperId == ol.HamperId).Image,
                                OrderId = ol.OrderId,
                                OrderLineId = ol.OrderLineId,
                                Price = ol.Price,
                                Quantity = ol.Quantity
                            }).ToList();

                            vm.TotalPrice = vm.OrderLines.Sum(ol => ol.Price);
                        }
                    }
                }
            }
            return View(vm);
        }

        [HttpGet]
        public IActionResult ViewHamper(string id, string rating)
        {
            HamperViewModel vm = new ViewModels.HamperViewModel();
            int hamperId = 0;
            int.TryParse(id, out hamperId);
                
            Hamper hamper = _hamperDataService.GetSingle(h => h.HamperId == hamperId);
            if (hamper != null)
            {
                vm.HamperId = hamperId;
                vm.Name = hamper.Name;
                vm.Image = hamper.Image;
                vm.Price = hamper.Price;
                vm.Quantity = 1;
                vm.CategoryId = hamper.CategoryId;
                vm.Details = hamper.Details;
                vm.Discontinue = hamper.Discontinue;
                vm.Rating = hamper.Rate;
            }

            if(!string.IsNullOrEmpty(rating))
            {
                int rate = Convert.ToInt32(rating);
                hamper.Rate = (int)Math.Ceiling((Convert.ToDouble(rate) + Convert.ToDouble(hamper.Rate)) / 2);
                _hamperDataService.Update(hamper);
                vm.Rating = hamper.Rate;
            }

            return View(vm);
        }

        [HttpPost]
        public IActionResult ViewHamper(HamperViewModel vm, string hamperIdString)
        {
            if (ModelState.IsValid)
            {
                int hamperId = int.Parse(hamperIdString);
                string cartId = ShoppingCartService.GetCartId(HttpContext);

                Order order = _orderDataService.GetSingle(o => o.CartId == cartId && !o.CheckedOut);
                if (order == null)
                {
                    order = new Order
                    {
                        CartId = cartId,
                        CheckedOut = false,
                        DateCreated = DateTime.Now,
                        Email = !string.IsNullOrWhiteSpace(User.Identity.Name) ? User.Identity.Name : ""
                    };
                    _orderDataService.Create(order);
                }

                OrderLine orderLine = _orderLineDataService.GetSingle(ol => ol.OrderId == order.OrderId && ol.HamperId == hamperId);
                if (orderLine == null)
                {
                    orderLine = new OrderLine
                    {
                        OrderId = order.OrderId,
                        HamperId = hamperId,
                        Quantity = 0,
                        Price = 0
                    };
                    _orderLineDataService.Create(orderLine);
                }

                orderLine.Quantity += vm.Quantity;
                orderLine.Price += (vm.Quantity * vm.Price);
                vm.Quantity = 1;

                _orderLineDataService.Update(orderLine);

            }

            return View(vm);
        }
    }
}
