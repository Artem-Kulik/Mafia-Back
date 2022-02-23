using go_mafia_back.Models.Configuration.Interfaces;
using go_mafia_back.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace go_mafia_back.Models.Configuration.Initializers
{
    public class PlayerRoleInitializer : ITypeInitializer
    {
        public async Task Init(ApplicationContext context)
        {
            if (context.PlayerRoles.Count() == 0) {
            PlayerRole[] roles = new PlayerRole[] {
                    new PlayerRole(){
                        Role = "Civilian"
                    },
                    new PlayerRole(){
                        Role = "Mafia"
                    },
                    new PlayerRole(){
                        Role = "DonMafia"
                    },
                    new PlayerRole(){
                        Role = "Sheriff"
                    }
                 };
                await context.Set<PlayerRole>().AddRangeAsync(roles);
            }
        }
    }
}
