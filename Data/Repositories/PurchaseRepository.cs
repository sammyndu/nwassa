using Nwassa.Core.Constants;
using Nwassa.Core.Data;
using Nwassa.Core.Purchased.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nwassa.Data.Repositories
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly IDataRepository _dataRepository;
        private readonly string _userTable;

        public PurchaseRepository(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
            _userTable = DatabaseCollectionConstants.PURCHASES_COLLECTION;
        }

        public List<PurchaseDocument> Get() =>
            _dataRepository.GetRecords<PurchaseDocument>(_userTable);

        public PurchaseDocument Get(Guid id) =>
            _dataRepository.GetRecordById<PurchaseDocument>(_userTable, id);


        public PurchaseDocument Create(PurchaseDocument purchaseDocument)
        {
            _dataRepository.InsertRecord(_userTable, purchaseDocument);
            return purchaseDocument;
        }


    }
}
