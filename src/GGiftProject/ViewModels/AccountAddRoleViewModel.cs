﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace GGiftProject.ViewModels
{
    public class AccountAddRoleViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}
