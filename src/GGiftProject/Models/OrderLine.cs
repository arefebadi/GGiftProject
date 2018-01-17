using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GGiftProject.Models
{
    public class OrderLine
    {
        public int OrderLineId { get; set; }
        public int OrderId { get; set; }
        public int HamperId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }




    }
}
