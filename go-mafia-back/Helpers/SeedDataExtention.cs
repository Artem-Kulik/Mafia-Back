using go_mafia_back.Models.Configuration.Interfaces;
using go_mafia_back.Models.Configuration;
using go_mafia_back.Models.Configuration.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace go_mafia_back.Helpers
{
    public static class SeedDataExtention
    {
        public static IWebHost SeedDatabase(this IWebHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                try
                {
                    var initializer = scope
                        .ServiceProvider
                        .GetRequiredService<IEntityInitializer>();

                    initializer.SeedData().Wait();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return webHost;
        }
    }
}
