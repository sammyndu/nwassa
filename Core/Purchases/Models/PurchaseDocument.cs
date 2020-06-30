using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nwassa.Core.Purchased.Models
{
    public class PurchaseDocument
    {
        public PurchaseDocument()
        {
            ProductIds = new List<Guid>();
        }

        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public List<Guid> ProductIds { get; set; }

        public DateTime DatePurchased { get; set; }
    }
}
