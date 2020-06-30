using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nwassa.Presentation.Models.Users
{
    public class AuthResponse
    {
       public UserInfo User { get; set; }

       public string Token { get; set; } 
    }
}
