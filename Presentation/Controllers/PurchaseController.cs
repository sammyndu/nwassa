using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Nwassa.Core.Purchased.Models;
using Nwassa.Core.Purchases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nwassa.Presentation.Controllers
{
    [Route("api/purchases")]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public PurchaseController(IPurchaseService purchaseService, IWebHostEnvironment hostingEnvironment)
        {
            _purchaseService = purchaseService;
            _hostingEnvironment = hostingEnvironment;
        }

        [Route("getall")]
        [HttpGet]
        public ActionResult<List<PurchaseDocument>> Get() =>
            _purchaseService.Get();

        [Route("get")]
        [HttpGet]
        public ActionResult<PurchaseDocument> Get(Guid id)
        {
            var purchase = _purchaseService.Get(id);

            if (purchase == null)
            {
                return NotFound();
            }

            return purchase;
        }

        [Route("create")]
        [HttpPost]
        public ActionResult<PurchaseDocument> Create(PurchaseDocument purchaseDocument)
        {
            _purchaseService.Create(purchaseDocument);

            return purchaseDocument;
        }
    }
}
