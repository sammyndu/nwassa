using Microsoft.AspNetCore.Http;
using Nwassa.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nwassa.Domain_Services
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            if (_httpContextAccessor.HttpContext.User?.Identity.Name != null)
            {
                UserId = Guid.Parse(_httpContextAccessor.HttpContext.User?.Identity.Name);
            }
        }
        public Guid? UserId { get; set; }
    }
}
