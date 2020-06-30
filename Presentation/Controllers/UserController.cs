using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Nwassa.Core.Users;
using Nwassa.Core.Users.Models;
using Nwassa.Domain_Services.Users;
using Nwassa.Presentation.Models.Products;
using Nwassa.Presentation.Models.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nwassa.Presentation.Controllers
{
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public UserController(IUserService userService, IWebHostEnvironment hostingEnvironment)
        {
            _userService = userService;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public ActionResult<List<UserDocument>> Get() =>
            _userService.GetAll();

        [HttpGet("{id:length(24)}")]
        public ActionResult<UserDocument> Get(Guid id)
        {
            var user = _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpGet("{id:length(24)}")]
        public ActionResult<UserDocument> Get(string email)
        {
            var user = _userService.GetByEmail(email);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost]
        public ActionResult<UserDocument> Create(UserDocument book)
        {
            _userService.Create(book);

            return book;
        }

        [Route("update")]
        [Authorize]
        [HttpPut]
        public IActionResult Update([FromBody] UserInfo userDoc)
        {
            var user = _userService.Get(userDoc.Id);

            if (user == null)
            {
                return NotFound("User not Found");
            }

            _userService.Update(userDoc.Id, userDoc);

            return NoContent();
        }

        [Route("updatefile")]
        [Authorize]
        [HttpPut]
        public IActionResult UpdateFile([FromForm] UpdateFilemodel model)
        {

            var file = Request.Form.Files[0];

            var user = _userService.Get(model.Id);

            if (user == null)
            {
                return NotFound("User not Found");
            }

            _userService.UpdateFile(model.Id, file);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(Guid id)
        {
            var book = _userService.Get(id);

            if (book == null)
            {
                return NotFound();
            }

            _userService.Remove(book.Id);

            return NoContent();
        }

        [Route("image")]
        [HttpGet]
        public IActionResult GetImage([FromQuery]string img, string id)
        {
            var filename = img;

            var path = Path.Combine(this._hostingEnvironment.ContentRootPath, "Upload", "Users", "PassportPhoto", id, filename);

            if (!System.IO.File.Exists(path))
            {
                return NotFound("File not found");
            }


            var b = System.IO.File.ReadAllBytes(path);
            return this.File(b, "image/jpg", filename);
        }
    }
}
