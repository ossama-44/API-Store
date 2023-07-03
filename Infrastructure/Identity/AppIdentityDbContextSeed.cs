using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new AppUser()
                {
                    DisplayName = "Osama",
                    Email = "osama.h.mohamed44@gmail.com",
                    UserName = "osama@gmail.com",
                    Address = new Address()
                    {
                        FistName = "Osama",
                        LastName = "Hassaneen",
                        Street = "60 st",
                        City = "Riyadh",
                        State = "Riyadh",
                        ZipCode = "12211"
                    }
                };

                await userManager.CreateAsync(user, "Password123#");
            }
        }
    }
}
