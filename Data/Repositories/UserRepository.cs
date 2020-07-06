using MongoDB.Driver;
using Nwassa.Core.Constants;
using Nwassa.Core.Data;
using Nwassa.Core.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nwassa.Data.Repositories
{
    public class UserRepository : IUserRepository 
    {
        private readonly IDataRepository _dataRepository;
        private readonly string _userTable;

        public UserRepository(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
            _userTable = DatabaseCollectionConstants.USERS_COLLECTION;
        }

        public List<UserDocument> Get() =>
            _dataRepository.GetRecords<UserDocument>(_userTable);

        public UserDocument Get(Guid id) =>
            _dataRepository.GetRecordById<UserDocument>(_userTable, id);

        public UserDocument Get(string email) =>
            _dataRepository.GetRecordByEmail<UserDocument>(_userTable, email);

        public UserDocument GetPhone(string phone) =>
            _dataRepository.GetRecordByPhone<UserDocument>(_userTable, phone);

        public UserDocument Create(UserDocument userDocument)
        {
            _dataRepository.InsertRecord(_userTable, userDocument);
            return userDocument;
        }

        public void Update(Guid id, UserDocument userDocument) =>
            _dataRepository.Upsert(_userTable, userDocument, id);

        public void Remove(Guid id) =>
            _dataRepository.Delete<UserDocument>(_userTable, id);

        public IMongoCollection<UserDocument> GetCollection()
        {
           return _dataRepository.GetCollection<UserDocument>(_userTable);
        }
    }
}
