using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nwassa.Core.Products.Models
{
    public class ProductDocument
    {
        public Guid Id { get; set; }

        public string Title { get; set; }
        
        public string Description { get; set; }

        public string Location { get; set; }

        public int Duration { get; set; }

        public int NumberOfUnits { get; set; }

        public string ProductPhoto { get; set; }

        public double Price { get; set; }

        public double SizeofFarm { get; set; }

        public double Roi { get; set; }

        public Guid Owner { get; set; }

        public DateTime DateCreated { get; set; }


    }
}
