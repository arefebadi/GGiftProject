using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GGiftProject.ViewModels
{
    public class HamperCheckoutViewModel
    {
        public int OrderId { get; set; }
        public string Email { get; set; }
        public string CartId { get; set; }
        public DateTime DateCreated { get; set; }
        public bool CheckedOut { get; set; }
        public double TotalPrice { get; set; }
        public List<HamperOrderLineViewModel> OrderLines { get; set; }
    }
}
