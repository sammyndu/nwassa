using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Nwassa.Core.Files.Models;
using Nwassa.Core.Products;
using Nwassa.Core.Products.Models;
using Nwassa.Presentation.Models.Products;

namespace Nwassa.Presentation.Controllers
{
	[Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
		private readonly IWebHostEnvironment _hostingEnvironment;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IProductService _productService;


		public ProductController(IWebHostEnvironment hostingEnvironment,
								IHttpContextAccessor httpContextAccessor,
								IProductService productService)
		{
			_hostingEnvironment = hostingEnvironment;
			_httpContextAccessor = httpContextAccessor;
			_productService = productService;
		}

		[Route("getall")]
		[HttpGet]
		public ActionResult<List<ProductDocument>> Get() =>
			_productService.Get();

		[Route("get")]
		[HttpGet]
		public ActionResult<ProductDocument> Get(Guid id)
		{
			var user = _productService.Get(id);

			if (user == null)
			{
				return NotFound();
			}

			return user;
		}

		[Route("create")]
		[Authorize]
		[HttpPost, DisableRequestSizeLimit]
		public ActionResult<ProductDocument> Create([FromForm] ProductModel model)
		{
			var image = Request.Form.Files[0];

			var product = JsonConvert.DeserializeObject<ProductDocument>(model.Product);
			
			return _productService.Create(product, image);
		}

		[Route("update")]
		[Authorize]
		[HttpPut]
		public IActionResult Update(ProductDocument product)
		{
			var existing_product = _productService.Get(product.Id);

			if (existing_product == null)
			{
				return NotFound();
			}

			_productService.Update(product.Id, product);

			return NoContent();
		}

		[Route("updatefile")]
		[Authorize]
		[HttpPut]
		public IActionResult UpdateFile([FromForm] UpdateFilemodel model)
		{
			var image = Request.Form.Files[0];

			var existing_product = _productService.Get(model.Id);

			if (existing_product == null)
			{
				return NotFound();
			}

			_productService.UpdateFile(model.Id, image);

			return NoContent();
		}

		[Route("delete")]
		[HttpDelete]
		public IActionResult Delete(Guid id)
		{
			var product = _productService.Get(id);

			if (product == null)
			{
				return NotFound();
			}

			_productService.Remove(product.Id);

			return NoContent();
		}

		[Route("upload")]
		[Authorize]
		[HttpPost, DisableRequestSizeLimit]
		public IActionResult UploadFile()
		{
			var allowedContentTypes = new string[] { "image/jpg", "image/png", "image/jpeg" };
			try
			{
				var file = Request.Form.Files[0];
				if (!allowedContentTypes.Contains(file.ContentType))
				{
					var username = _httpContextAccessor.HttpContext.User.Identity.Name;
					return BadRequest("Invalid file extension "+ username);
				}
				string folderName = "Upload";
				string webRootPath = _hostingEnvironment.ContentRootPath;
				string newPath = Path.Combine(webRootPath, folderName);
				if (!Directory.Exists(newPath))
				{
					Directory.CreateDirectory(newPath);
				}
				if (file.Length > 0)
				{
					string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
					string fullPath = Path.Combine(newPath, fileName);
					using (var stream = new FileStream(fullPath, FileMode.Create))
					{
						file.CopyTo(stream);
					}
				}
				return Ok("Upload Successful.");
			}
			catch (System.Exception ex)
			{
				return StatusCode(500, "Upload Failed, " + ex.Message);
			}
		}

		[Route("image")]
		[HttpGet]
		public IActionResult GetImage([FromQuery]string img, string id)
		{
			var filename = img;

			var path = Path.Combine(this._hostingEnvironment.ContentRootPath, "Upload", "Products", id, filename);

			if (!System.IO.File.Exists(path))
			{
				return NotFound("File not found");
			}


			var b = System.IO.File.ReadAllBytes(path);
			return this.File(b, "image/jpg", filename);
		}
	}
}