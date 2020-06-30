using Microsoft.AspNetCore.Mvc;
using Nwassa.Core.Accounts;
using Nwassa.Core.Accounts.Models;
using Nwassa.Core.Users;
using Nwassa.Core.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Nwassa.Core.Helpers;
using Microsoft.Extensions.Options;
using Nwassa.Presentation.Models.Users;
using Nwassa.Core.Data;
using MongoDB.Driver;
using System.Security.Authentication;
using static Nwassa.Core.Helpers.EmailGenerator;

namespace Nwassa.Domain_Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly IUserService _userService;
        private readonly AppSettings _appSettings;
        private readonly IUserRepository _userRepository;
        private readonly ICrypto _crypto;
        private readonly IEmailGenerator _emailGenerator;

        public AccountService(IUserService userService,
                        IOptions<AppSettings> appSettings,
                        IUserRepository userRepository,
                        ICrypto crypto,
                        IEmailGenerator emailGenerator)
        {
            _userService = userService;
            _appSettings = appSettings.Value;
            _userRepository = userRepository;
            _crypto = crypto;
            _emailGenerator = emailGenerator;
        }

        public AuthResponse Login(LoginModel login)
        {
            UserDocument user; 
            if (IsPhoneNumber(login.Username))           
            {
                var userCollection = _userRepository.GetCollection();
                var users = userCollection.Find(x => x.PhoneNumber == login.Username);
                user = users.FirstOrDefault();
            }else
            {
                user = _userService.GetByEmail(login.Username);
            }
            if (user == null)
            {
                throw new InvalidCredentialException("Invalid Email/Password combination.");
            } 


            var encrptedPassword = _crypto.Hash(login.Password, user.LoginProfile.Salt, 5523);

            if (user.LoginProfile.Password != encrptedPassword)
            {
                throw new InvalidCredentialException("Invalid Email/Password combination.");
            }

            if (user != null)
            {
                var userInfo = new UserInfo
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    PassportPhoto = user.PassportPhoto,
                    BVN = user.BVN,
                    DateCreated = user.DateCreated
                };
                var token = GenerateJwtToken(userInfo);
                return new AuthResponse { User = userInfo, Token = token };
            }

            return null ;

        }

        public AuthResponse Register(RegisterModel register)
        {
            var userDocument = new UserDocument {
                                Id = Guid.NewGuid(),
                                Email = register.Email,
                                FirstName = register.FirstName,
                                LastName = register.LastName,
                                PhoneNumber = register.PhoneNumber,
                                DateCreated = DateTime.UtcNow };

            var emailModel = new EmailModel
            {
                Reciever = userDocument.Email
            };

            //var emailVerify = _emailGenerator.SendMail(emailModel);

            var salt = _crypto.GenerateSalt(32);

            var encrptedPassword = _crypto.Hash(register.Password, salt, 5523);

            userDocument.LoginProfile = new LoginProfile();

            userDocument.LoginProfile.Password = encrptedPassword;

            userDocument.LoginProfile.Salt = salt;

            var user = _userService.Create(userDocument);

            if (user != null)
            {
                var userInfo = new UserInfo
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    PassportPhoto = user.PassportPhoto,
                    BVN = user.BVN,
                    DateCreated = user.DateCreated
                };
                var token = GenerateJwtToken(userInfo);
                return new AuthResponse { User = userInfo, Token = token };
            }

            throw new UnauthorizedAccessException("User not Created"); ;
        }

        public void ChangePassword(ChangePasswordModel model)
        {
            var user = _userService.GetByEmail(model.Email);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User not Found");
            }

            var encrptedPassword = _crypto.Hash(model.CurrentPassword, user.LoginProfile.Salt, 5523);

            if (user.LoginProfile.Password != encrptedPassword)
            {
                throw new InvalidCredentialException("Invalid Password.");
            }

            var salt = _crypto.GenerateSalt(32);

            user.LoginProfile.Password = _crypto.Hash(model.NewPassword, salt, 5523);
            user.LoginProfile.Salt = salt;

            _userRepository.Update(user.Id, user);
        }

        private string GenerateJwtToken(UserInfo user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool IsPhoneNumber(string username)
        {
            foreach (char c in username)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }
    }
}
