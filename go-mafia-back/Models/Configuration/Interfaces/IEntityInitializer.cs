using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace go_mafia_back.Models.Configuration.Interfaces
{
    public interface IEntityInitializer
    {
        Task SeedData();
        void AddConfig(ITypeInitializer typeInitializer);
    }
}
