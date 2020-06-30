using Nwassa.Core.Products.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nwassa.Core.Data
{
    public interface IProductRepository
    {
        List<ProductDocument> Get();

        ProductDocument Get(Guid id);

        ProductDocument Create(ProductDocument ProductDocument);

        void Update(Guid id, ProductDocument userIn);

        void Remove(Guid id);
    }
}
