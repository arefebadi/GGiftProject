using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using GGiftProject.Models;
using Microsoft.EntityFrameworkCore;

namespace GGiftProject.Services
{
    public class MyDbContext:IdentityDbContext
    {
        public DbSet<Profile> TblProfile { get; set; }
        public DbSet<Address> TblAddress { get; set; }
        public DbSet<Category> TblCategory { get; set; }
        public DbSet<Hamper> TblHamper { get; set; }
        public DbSet<Order> TblOrder { get; set; }
        public DbSet<OrderLine> TblOrderLine{ get; set; }
      

        protected override void OnConfiguring(DbContextOptionsBuilder option)
        {
            option.UseSqlServer(@"Server=(localdb)\mssqllocaldb; Database=GGiftDatabase; Trusted_Connection=true");
        }
    }
}
