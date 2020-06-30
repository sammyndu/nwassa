using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Nwassa.Core.Data;
using Nwassa.Core.Files;
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
		private readonly CloudinaryMetaData _cloudinaryMetaData;

		public ProductService(IProductRepository productRepository,
			IUserContext userContext, IWebHostEnvironment hostingEnvironment, CloudinaryMetaData cloudinaryMetaData)
        {
            _productRepository = productRepository;
			_userContext = userContext;
			_hostingEnvironment = hostingEnvironment;
			_cloudinaryMetaData = cloudinaryMetaData;
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
				//string folderName = "Upload";
				//string webRootPath = _hostingEnvironment.ContentRootPath;
				//string newPath = Path.Combine(webRootPath, folderName, "Products", $"{productDocument.Id}");
				//if (!Directory.Exists(newPath))
				//{
				//	Directory.CreateDirectory(newPath);
				//}
				if (file.Length > 0)
				{
					CloudinaryDotNet.Account account = new CloudinaryDotNet.Account(
							_cloudinaryMetaData.CloudName,
							_cloudinaryMetaData.ApiKey,
							_cloudinaryMetaData.ApiSecret);

					Cloudinary cloudinary = new Cloudinary(account);

					var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.RemoveSpecialCharacters().Trim('"');

					var uploadParams = new ImageUploadParams()
					{
						File = new FileDescription(fileName, file.OpenReadStream())
					};
					var uploadResult = cloudinary.Upload(uploadParams);

					//string fullPath = Path.Combine(newPath, fileName);
					//using (var stream = new FileStream(fullPath, FileMode.Create))
					//{
					//	file.CopyTo(stream);
					//}
					productDocument.ProductPhoto = uploadResult.Url.AbsoluteUri;
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
				//string folderName = "Upload";
				//string webRootPath = _hostingEnvironment.ContentRootPath;
				//string newPath = Path.Combine(webRootPath, folderName, "Products", $"{productDocument.Id}");
				//if (!Directory.Exists(newPath))
				//{
				//	Directory.CreateDirectory(newPath);
				//}
				if (file.Length > 0)
				{
					CloudinaryDotNet.Account account = new CloudinaryDotNet.Account(
							_cloudinaryMetaData.CloudName,
							_cloudinaryMetaData.ApiKey,
							_cloudinaryMetaData.ApiSecret);

					Cloudinary cloudinary = new Cloudinary(account);

					var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.RemoveSpecialCharacters().Trim('"');

					var uploadParams = new ImageUploadParams()
					{
						File = new FileDescription(fileName, file.OpenReadStream())
					};
					var uploadResult = cloudinary.Upload(uploadParams);

					//string fullPath = Path.Combine(newPath, fileName);
					//using (var stream = new FileStream(fullPath, FileMode.Create))
					//{
					//	file.CopyTo(stream);
					//}
					product.ProductPhoto = uploadResult.Url.AbsoluteUri;
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
