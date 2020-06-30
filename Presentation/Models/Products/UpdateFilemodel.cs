using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nwassa.Presentation.Models.Products
{
    public class UpdateFilemodel
    {
        public IFormFile File { get; set; }

        public Guid Id { get; set; }
    }
}
