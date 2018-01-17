using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GGiftProject.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string CartId { get; set; }
        public string Email { get; set; }
        public DateTime DateCreated { get; set; }
        public bool CheckedOut { get; set; }


    }
}
