using Microsoft.AspNetCore.Hosting;
using Nwassa.Core.Data;
using Nwassa.Core.Purchased.Models;
using Nwassa.Core.Purchases;
using Nwassa.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nwassa.Domain_Services
{
    public class PurchaseService : IPurchaseService
    {
		private readonly IPurchaseRepository _purchaseRepository;
		private readonly IProductRepository _productRepository;
		private readonly IUserContext _userContext;
		private readonly IWebHostEnvironment _hostingEnvironment;

		public PurchaseService(IPurchaseRepository purchaseRepository,
			IProductRepository productRepository,
			IUserContext userContext, IWebHostEnvironment hostingEnvironment)
		{
			_purchaseRepository = purchaseRepository;
			_userContext = userContext;
			_hostingEnvironment = hostingEnvironment;
			_productRepository = productRepository;
		}

		public List<PurchaseDocument> Get() =>
			_purchaseRepository.Get();

		public PurchaseDocument Get(Guid id) =>
			_purchaseRepository.Get(id);

		public PurchaseDocument Create(PurchaseDocument purchaseDocument)
		{
			purchaseDocument.Id = Guid.NewGuid();
			purchaseDocument.DatePurchased = DateTime.UtcNow;
			purchaseDocument.UserId = _userContext.UserId.Value;

			foreach (var productId in purchaseDocument.ProductIds)
			{
				var product = _productRepository.Get(productId);

				if (product.NumberOfUnits == 0)
				{
					throw new UnauthorizedAccessException("No available Units for this product");
				}

				product.NumberOfUnits -= 1;

				_productRepository.Update(productId, product);
			}

			return _purchaseRepository.Create(purchaseDocument);
		}
	}
}
