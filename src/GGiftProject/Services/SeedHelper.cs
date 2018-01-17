using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GGiftProject.Services
{
    public static class SeedHelper
    {
        public static async Task Seed(IServiceProvider serviceProvider)
        {
            RoleManager<IdentityRole> roleManger = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            //seed the database with Admin role
            if (await roleManger.FindByNameAsync("Admin") == null)
            {
                await roleManger.CreateAsync(new IdentityRole("Admin"));
            }

            //seed the database with Customer role
            if (await roleManger.FindByNameAsync("Customer") == null)
            {
                await roleManger.CreateAsync(new IdentityRole("Customer"));
            }
        }
    }
}
