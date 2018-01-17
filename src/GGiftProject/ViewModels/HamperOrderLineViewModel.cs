using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GGiftProject.ViewModels
{
    public class HamperOrderLineViewModel
    {
        public int OrderLineId { get; set; }
        public int HamperId { get; set; }
        public int OrderId { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string HamperName { get; set; }
        public string Image { get; set; }
    }
}
