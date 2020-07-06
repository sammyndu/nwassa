using MongoDB.Driver;
using Nwassa.Core.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nwassa.Core.Data
{
    public interface IUserRepository
    {
        List<UserDocument> Get();

        UserDocument Get(Guid id);

        UserDocument Get(string email);

        UserDocument GetPhone(string phone);

        UserDocument Create(UserDocument UserDocument);

        void Update(Guid id, UserDocument userIn);

        void Remove(Guid id);

        IMongoCollection<UserDocument> GetCollection();

    }
}
