using go_mafia_back.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace go_mafia_back.Models.Configuration.Interfaces
{
    public interface ITypeInitializer
    {
        Task Init(ApplicationContext context);
    }
}
