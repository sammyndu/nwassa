using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nwassa.Core.Data
{
    public interface IDataRepository
    {
        void InsertRecord<T>(string table, T record);

        List<T> GetRecords<T>(string table);

        T GetRecordById<T>(string table, Guid id);

        T GetRecordByEmail<T>(string table, string email);

        Guid Upsert<T>(string table, T record, Guid id);

        void Delete<T>(string table, Guid id);

        IMongoCollection<T> GetCollection<T>(string table);
    }
}
