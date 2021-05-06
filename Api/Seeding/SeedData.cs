using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Seeding
{
    public static class SeedData
    {
        private static ApplicationDbContext _context { get; set; }
        private static UserManager<IdentityUser> _userManager { get; set; }

        public async static Task Start(IServiceProvider services)
        {
            _context = services.GetRequiredService<ApplicationDbContext>();
            _userManager = services.GetRequiredService<UserManager<IdentityUser>>();


            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();

            await _userManager.CreateAsync(new User
            {
                FirstName = "FirstName",
                LastName = "Surname",
                UserName = "DemoUser",
                Email = "demo@user.com"
            }, "Passw0rd!#");
        }
    }
}
