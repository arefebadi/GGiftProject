using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GGiftProject.ViewModels
{
    public class HamperHampersListViewModel
    {
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
       
        public List<HamperViewModel> Hampers { get; set; }
        public List<HamperCategoryViewModel> Categories { get; set; }
    }
}
