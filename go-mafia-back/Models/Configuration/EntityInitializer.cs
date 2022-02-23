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

            this.AddConfig(new PlayerRoleInitializer());
        }

        public void AddConfig(ITypeInitializer typeInitializer)
        {
            typeInitializers.Add(typeInitializer);
        }

        public async Task SeedData()
        {
            //always delete and recreate database with seeded data
            //bool deleted = await context.Database.EnsureDeletedAsync();

            bool created = await context.Database.EnsureCreatedAsync();

            InitializeIdetity();

            foreach (var initializer in typeInitializers)
            {
                await initializer.Init(context);
                await context.SaveChangesAsync();
            }
        }

        private void InitializeIdetity()
        {
            if (roleManager.FindByNameAsync("Guest").Result == null)
            {
                var resultAdminRole = roleManager.CreateAsync(new IdentityRole
                {
                    Name = "Admin"
                }).Result;
                var resultUserRole = roleManager.CreateAsync(new IdentityRole
                {
                    Name = "User"
                }).Result;
                var resultGuestRole = roleManager.CreateAsync(new IdentityRole
                {
                    Name = "Guest"
                }).Result;
            }
            if (userManager.Users.Count() == 0)
            {
                string email = "user@gmail.com";
                var guest = new User
                {
                    Email = email,
                    UserName = email
                };
                var resultGuest = userManager.CreateAsync(guest, "Qwerty1-").Result;
                resultGuest = userManager.AddToRoleAsync(guest, "Guest").Result;
            }
        }
    }
}
