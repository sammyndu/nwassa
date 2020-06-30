using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Nwassa.Core.Data;
using Nwassa.Core.Helpers;
using Nwassa.Core.Products;
using Nwassa.Core.Products.Models;
using Nwassa.Core.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Nwassa.Domain_Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
		private readonly IUserContext _userContext;
		private readonly IWebHostEnvironment _hostingEnvironment;

		public ProductService(IProductRepository productRepository,
			IUserContext userContext, IWebHostEnvironment hostingEnvironment)
        {
            _productRepository = productRepository;
			_userContext = userContext;
			_hostingEnvironment = hostingEnvironment;
		}

        public List<ProductDocument> Get() =>
            _productRepository.Get();

        public ProductDocument Get(Guid id) =>
            _productRepository.Get(id);

        public ProductDocument Create(ProductDocument productDocument, IFormFile image)
        {
			productDocument.Id = Guid.NewGuid();
			productDocument.DateCreated = DateTime.UtcNow;
			productDocument.Owner = _userContext.UserId.Value;

			var allowedContentTypes = new string[] { "image/jpg", "image/png", "image/jpeg" };
			try
			{
				var file = image;
				Guid username = _userContext.UserId.Value;
				if (!allowedContentTypes.Contains(file.ContentType))
				{
					throw new InvalidOperationException("Invalid image extension");
				}
				string folderName = "Upload";
				string webRootPath = _hostingEnvironment.ContentRootPath;
				string newPath = Path.Combine(webRootPath, folderName, "Products", $"{productDocument.Id}");
				if (!Directory.Exists(newPath))
				{
					Directory.CreateDirectory(newPath);
				}
				if (file.Length > 0)
				{
					var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.RemoveSpecialCharacters().Trim('"');
					string fullPath = Path.Combine(newPath, fileName);
					using (var stream = new FileStream(fullPath, FileMode.Create))
					{
						file.CopyTo(stream);
					}
					productDocument.ProductPhoto = fileName;
				}
				return _productRepository.Create(productDocument);

			}
			catch (System.Exception ex)
			{
				throw new Exception("Upload Failed, " + ex.Message);
			}
			
        }

        public void Update(Guid id, ProductDocument productDocument) =>
            _productRepository.Update(id, productDocument);

		public void UpdateFile(Guid productId, IFormFile image)
		{
			var product = Get(productId);

			var allowedContentTypes = new string[] { "image/jpg", "image/png", "image/jpeg" };
			try
			{
				var file = image;
				Guid username = _userContext.UserId.Value;
				if (!allowedContentTypes.Contains(file.ContentType))
				{
					throw new InvalidOperationException("Invalid image extension");
				}
				string folderName = "Upload";
				string webRootPath = _hostingEnvironment.ContentRootPath;
				string newPath = Path.Combine(webRootPath, folderName, "Products", $"{productId}");
				if (!Directory.Exists(newPath))
				{
					Directory.CreateDirectory(newPath);
				}
				if (file.Length > 0)
				{
					var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.RemoveSpecialCharacters().Trim('"');
					string fullPath = Path.Combine(newPath, fileName);
					using (var stream = new FileStream(fullPath, FileMode.Create))
					{
						file.CopyTo(stream);
					}
					product.ProductPhoto = fileName;
				}

				_productRepository.Update(productId, product);

			}
			catch (System.Exception ex)
			{
				throw new Exception("Upload Failed, " + ex.Message);
			}

		}

		public void Remove(Guid id) =>
            _productRepository.Remove(id);

    }
}
