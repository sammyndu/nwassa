using Microsoft.AspNetCore.Http;
using Nwassa.Core.Products.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nwassa.Core.Products
{
    public interface IProductService
    {
        List<ProductDocument> Get();

        ProductDocument Get(Guid id);

        ProductDocument Create(ProductDocument userDocument, IFormFile image);

        void Update(Guid id, ProductDocument userDocument);

        void UpdateFile(Guid productId, IFormFile image);

        void Remove(Guid id);
    }
}
