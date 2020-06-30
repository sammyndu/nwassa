using Microsoft.AspNetCore.Http;
using Nwassa.Core.Users.Models;
using Nwassa.Presentation.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nwassa.Core.Users
{
    public interface IUserService
    {
        List<UserDocument> GetAll();

        UserDocument Get(Guid id);

        UserDocument GetByEmail(string email);

        UserDocument Create(UserDocument userDocument);

        void Update(Guid id, UserInfo userDocument);

        void UpdateFile(Guid Id, IFormFile file);

        void Remove(Guid id);
    }
}
