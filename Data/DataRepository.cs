using MongoDB.Bson;
using MongoDB.Driver;
using Nwassa.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Nwassa.Data.Models.NwassaDatabaseSettings;

namespace Nwassa.Data
{
    public class DataRepository : IDataRepository
    {
        private readonly IMongoDatabase db;
        private readonly MongoCollectionSettings collectionSettings;

        public DataRepository(INwassaDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            db = client.GetDatabase(settings.DatabaseName);
            collectionSettings = new MongoCollectionSettings
            {
                GuidRepresentation = GuidRepresentation.Standard
            };
        }

        public void InsertRecord<T>(string table, T record)
        {

            var collection = db.GetCollection<T>(table, collectionSettings);
            collection.InsertOne(record);
        }

        public IMongoCollection<T> GetCollection<T>(string table)
        {
            var collection = db.GetCollection<T>(table);
            return collection;
        }

        public List<T> GetRecords<T>(string table)
        {
            var collection = db.GetCollection<T>(table);
            return collection.Find(x => true).ToList();
        }

        public T GetRecordById<T>(string table, Guid id)
        {
            var collection = db.GetCollection<T>(table, collectionSettings);
            var filter = Builders<T>.Filter.Eq("Id", id);

            var user = collection.Find(filter).FirstOrDefault();
            return user;
        }

        public T GetRecordByEmail<T>(string table, string email)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Email", email);

            return collection.Find(filter).FirstOrDefault();
        }

        public Guid Upsert<T>(string table, T record, Guid id)
        {
            var collection = db.GetCollection<T>(table, collectionSettings);
            var filter = Builders<T>.Filter.Eq("Id", id);
            collection.ReplaceOne(
                filter,
                record,
                new ReplaceOptions { IsUpsert = false });

            return id;
        }

        public void Delete<T>(string table, Guid id)
        {
            var collection = db.GetCollection<T>(table, collectionSettings);
            var filter = Builders<T>.Filter.Eq("Id", id);

            collection.DeleteOne(filter);
        }
    }
}
