using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace GGiftProject.ViewModels
{
    public class AccountRegisterViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required,DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required, DataType(DataType.Password)]

        public string Password { get; set; }
        [Required, DataType(DataType.Date)]

        public DateTime DOB { get; set; }
        [Required, DataType(DataType.Password),Compare(nameof(Password))]

        public string ConfirmPassword{ get; set; }
        
    }
}
