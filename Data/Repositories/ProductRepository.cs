using Nwassa.Core.Constants;
using Nwassa.Core.Data;
using Nwassa.Core.Products.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nwassa.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDataRepository _dataRepository;
        private readonly string _userTable;

        public ProductRepository(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
            _userTable = DatabaseCollectionConstants.PRODUCTS_COLLECTION;
        }

        public List<ProductDocument> Get() =>
            _dataRepository.GetRecords<ProductDocument>(_userTable);

        public ProductDocument Get(Guid id) =>
            _dataRepository.GetRecordById<ProductDocument>(_userTable, id);


        public ProductDocument Create(ProductDocument productDocument)
        {
            _dataRepository.InsertRecord(_userTable, productDocument);
            return productDocument;
        }

        public void Update(Guid id, ProductDocument productDocument) =>
            _dataRepository.Upsert(_userTable, productDocument, id);

        public void Remove(Guid id) =>
            _dataRepository.Delete<ProductDocument>(_userTable, id);
    }
}
