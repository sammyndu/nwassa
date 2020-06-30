using Nwassa.Core.Purchased.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nwassa.Core.Data
{
    public interface IPurchaseRepository
    {
        List<PurchaseDocument> Get();

        PurchaseDocument Get(Guid id);

        PurchaseDocument Create(PurchaseDocument purchaseDocument);

    }
}
