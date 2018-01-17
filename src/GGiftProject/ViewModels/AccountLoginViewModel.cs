using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GGiftProject.ViewModels
{
    public class AccountLoginViewModel
    {
        [Required,DataType(DataType.EmailAddress)]
        public string Email{ get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

    }
}
