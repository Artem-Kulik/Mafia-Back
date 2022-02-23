using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using go_mafia_back.Models.Dto.ResultDto;
using go_mafia_back.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace go_mafia_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public GameController(ApplicationContext context)
        {
            _context = context;
        }
        [HttpGet]
        public ResultDto getRoles()
        {
            var roles = _context.PlayerRoles.Select(x => new PlayerRole
            {
                Id = x.Id,
                Role = x.Role
            }).ToList();

            return new CollectionResultDto<PlayerRole>()
            {
                IsSuccessful = true,
                Data = roles
            };

        }

    }
}