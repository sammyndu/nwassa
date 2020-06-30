using Microsoft.AspNetCore.Mvc;
using Nwassa.Core.Accounts;
using Nwassa.Core.Accounts.Models;
using Nwassa.Core.Users.Models;
using Nwassa.Presentation.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nwassa.Presentation.Controllers
{
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            var response =  _accountService.Login(login);
            if (response !=  null)
            {
                return Ok(response);
            }
            return BadRequest(new { message = "Username or password is incorrect" });
        }

        [HttpPost("register")]
        public AuthResponse Register([FromBody] RegisterModel register)
        {
            return _accountService.Register(register);
        }

        [HttpPost("changepassword")]
        public void ChangePassword([FromBody] ChangePasswordModel model)
        {
            _accountService.ChangePassword(model);
        }
    }
}
