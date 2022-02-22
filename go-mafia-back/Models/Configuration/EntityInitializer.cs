using go_mafia_back.Models.Configuration.Initializers;
using go_mafia_back.Models.Configuration.Interfaces;
using go_mafia_back.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace go_mafia_back.Models.Configuration
{
    public class EntityInitializer : IEntityInitializer
    {
        private readonly List<ITypeInitializer> typeInitializers;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationContext context;

        public EntityInitializer(UserManager<User> _userManager,
                                 RoleManager<IdentityRole> _roleManager,
                                 ApplicationContext _context)
        {
            typeInitializers = new List<ITypeInitializer>();
            userManager = _userManager;
            roleManager = _roleManager;
            context = _context;

            //add concrete initializers
            //order is IMPORTANT (for example, you first need to add categories and only then products that are category-dependent)

            this.AddConfig(new PlayerRoleInitializer());
        }

        public void AddConfig(ITypeInitializer typeInitializer)
        {
            typeInitializers.Add(typeInitializer);
        }

        public async Task SeedData()
        {
            //always delete and recreate database with seeded data
            bool deleted = await context.Database.EnsureDeletedAsync();
            bool created = await context.Database.EnsureCreatedAsync();
            await InitializeIdetity();
            //create test users and admins
            //go through all the initializers and seed them all
            foreach (var initializer in typeInitializers)
            {
                await initializer.Init(context);
                await context.SaveChangesAsync();
            }
        }

        private async Task InitializeIdetity()
        {
            //Create roles
            await roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
            await roleManager.CreateAsync(new IdentityRole { Name = "User" });
            await roleManager.CreateAsync(new IdentityRole { Name = "Guest" });

            await userManager.CreateAsync(new User
            {
                UserName = "user@gmail.com",
                Email = "user@gmail.com"                
            }, "Qwerty1-");

            var guest = await userManager.FindByEmailAsync("user@gmail.com");
            await userManager.AddToRoleAsync(guest, "Guest");
        }
    }
}
