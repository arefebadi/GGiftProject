using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GGiftProject.Models
{
    public class Profile
    {
        public int ProfileId { get; set; }
        public string FirstName{ get; set; }

        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DOB { get; set; }
        public bool IsAdmin { get; set; }
        public List<Address> Addresses { get; set; }

    }
}
