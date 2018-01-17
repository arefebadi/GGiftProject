using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GGiftProject.ViewModels
{
    public class HamperAddViewModel
    {
        public int HamperId { get; set; }
        public string Name { get; set; }
       
        public double Price { get; set; }
        public string Details { get; set; }
        public bool Discontinue { get; set; }

        public string Image { get; set; }

        public int CategoryId { get; set; }

        public List<HamperCategoryViewModel> Categories { get; set; }
    }
}
