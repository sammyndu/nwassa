using Microsoft.AspNetCore.Http;
using Nwassa.Core.Products.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nwassa.Presentation.Models.Products
{
    public class ProductModel
    {
        public string Product { get; set; }

        public IFormFile File { get; set; }
    }
}
