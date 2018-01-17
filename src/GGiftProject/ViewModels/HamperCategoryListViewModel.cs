using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GGiftProject.ViewModels
{
    public class HamperCategoryListViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<HamperCategoryViewModel> Categories { get; set; }
    }
}
