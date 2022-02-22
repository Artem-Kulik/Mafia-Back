using go_mafia_back.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace go_mafia_back.Services.Interfaces
{
    public interface IJwtTokenService
    {
        string CreateToken(User user);
    }
}

