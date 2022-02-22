using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace go_mafia_back.Models.Dto
{
    public class RegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
    }
}
