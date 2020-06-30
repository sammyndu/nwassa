using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nwassa.Core.Users
{
    public interface IUserContext
    {
        public Guid? UserId { get; set; }
    }
}
