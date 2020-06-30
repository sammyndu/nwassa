using Nwassa.Core.Purchased.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nwassa.Core.Purchases
{
    public interface IPurchaseService
    {
        List<PurchaseDocument> Get();

        PurchaseDocument Get(Guid id);

        PurchaseDocument Create(PurchaseDocument purchaseDocument);
    }
}
