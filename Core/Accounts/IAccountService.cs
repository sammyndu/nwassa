using Nwassa.Core.Accounts.Models;
using Nwassa.Core.Users.Models;
using Nwassa.Presentation.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nwassa.Core.Accounts
{
    public interface IAccountService
    {
        AuthResponse Login(LoginModel login);

        AuthResponse Register(RegisterModel register);

        void ChangePassword(ChangePasswordModel model);
    }
}
