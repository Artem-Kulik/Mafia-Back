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
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Hosting;

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
        IHostingEnvironment env = null;

        public AccountController(
                ApplicationContext context,
                UserManager<User> userManager,
                SignInManager<User> signInManager,
                IJwtTokenService jwtTokenService,
                IHostingEnvironment env)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
            this.env = env;
        }

        [HttpGet]
        public ResultDto getUsers()
        {
            var users = _context.Users.Select(x => new UserAdditionalInfo
            {
               Name = x.Email
            }).ToList();

            return new CollectionResultDto<UserAdditionalInfo>()
            {
                IsSuccessful = true,
                Data = users
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
                await _userManager.AddToRoleAsync(user, "User");
                _context.SaveChanges();

                //MimeMessage message = new MimeMessage();

                //MailboxAddress from = new MailboxAddress("Artem",
                //"art.kulik2005@gmail.com");
                //message.From.Add(from);

                //MailboxAddress to = new MailboxAddress("User",
                //"nasara.my.ten@gmail.com");
                //message.To.Add(to);

                //message.Subject = "GoMafia";

                //BodyBuilder bodyBuilder = new BodyBuilder();
                //bodyBuilder.HtmlBody = "<h1>My message</h1>";
                //bodyBuilder.TextBody = "My message";

                //message.Body = bodyBuilder.ToMessageBody();


                //SmtpClient client = new SmtpClient();
                //client.Connect("smtp_address_here", port_here, true);
                //client.Authenticate("user_name_here", "pwd_here");


                //client.Send(message);
                //client.Disconnect(true);
                //client.Dispose();
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
