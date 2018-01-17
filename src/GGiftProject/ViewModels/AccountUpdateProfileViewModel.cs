using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GGiftProject.ViewModels
{
    public class AccountUpdateProfileViewModel
    {
        public int ProfileId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        //[Required, DataType(DataType.Password)]
        //public string Password { get; set; }
        //[Required, DataType(DataType.Password), Compare(nameof(Password))]
        //public string ConfirmPassword { get; set; }
        [Required, DataType(DataType.Date)]
        public DateTime DOB { get; set; }
        //public bool IsAdmin { get; set; }
        public List<AccountAddressViewModel> Addresses { get; set; }


    }
}
