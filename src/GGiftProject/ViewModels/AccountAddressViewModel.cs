﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GGiftProject.ViewModels
{
    public class AccountAddressViewModel
    {
        public int AddressId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public int PostCode { get; set; }
        public bool IsFavourite { get; set; }
    }
}
