using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using go_mafia_back.Models.Dto;
using go_mafia_back.Models.Dto.ResultDto;
using go_mafia_back.Models.Entities;
using go_mafia_back.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace go_mafia_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;

        public AccountController(
                ApplicationContext context,
                UserManager<User> userManager,
                SignInManager<User> signInManager,
                IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
        }
        [HttpGet]
        public ResultDto ok()
        {
            return new ResultDto
            {
                IsSuccessful = true,
                Message = "Seccess go)))"
            };
        }

        [HttpPost("register")]
        public async Task<ResultDto> Register(RegisterDto model)
        {
            if (model.Name == null || model.Name == "")
            {
                return new ResultDto
                {
                    IsSuccessful = false,
                    Message = "Name is empty"
                };
            }
            if (model.Password == null || model.Password == "")
            {
                return new ResultDto
                {
                    IsSuccessful = false,
                    Message = "Password is empty"
                };
            }
            if (model.Email == null || model.Email == "")
            {
                return new ResultDto
                {
                    IsSuccessful = false,
                    Message = "Email is empty"
                };
            }
            if (_userManager.Users.Any(x => x.Email == model.Email) == true)
            {
                return new ResultDto
                {
                    IsSuccessful = false,
                    Message = '~' + model.Email + "~ already registered"
                };
            }
            User user = new User()
            {
                Email = model.Email,
                UserName = model.Email,
                PasswordHash = model.Password // my
            };
            //await _userManager.CreateAsync(user, model.Password);

            UserAdditionalInfo ui = new UserAdditionalInfo()
            {
                User = user,
                Id = user.Id,
                Name = model.Name,
                Photo = model.Photo
            };
            try
            {
                _context.UserAdditionalInfo.Add(ui);
                _context.SaveChanges();
                await _userManager.AddToRoleAsync(user, "User");  // ???
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return new ResultDto
                {
                    IsSuccessful = false,
                    Message = ex.Message
                };
            }
            return new ResultDto
            {
                IsSuccessful = true
            };
        }

        [HttpPost("login")]
        public async Task<ResultDto> Login(LoginDto model)
        {
            //var res = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            //if (!res.Succeeded)
            //{
            //    return new ResultDto
            //    {
            //        IsSuccessful = false
            //    };
            //}            
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                if (user.PasswordHash == model.Password)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    var id = _context.Users.Where(el => el.Email == model.Email).FirstOrDefault().Id;

                    string t = _jwtTokenService.CreateToken(user);
                    return new ResultLoginDto
                    {
                        IsSuccessful = true,
                        Token = t,
                        Message = user.Email
                    };
                }
                else
                {
                    return new ResultDto
                    {
                        IsSuccessful = false,
                        Message ="Wrong password"
                    };
                }
            }
            else
            {
                return new ResultDto
                {
                    IsSuccessful = false,
                    Message ="Wrong email"
                };
            }
        }
    }
}
