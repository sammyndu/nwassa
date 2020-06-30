using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Nwassa.Core.Constants;
using Nwassa.Core.Data;
using Nwassa.Core.Helpers;
using Nwassa.Core.Users;
using Nwassa.Core.Users.Models;
using Nwassa.Presentation.Models.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Nwassa.Domain_Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserContext _userContext;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public UserService(IUserRepository userRepository, IUserContext userContext, IWebHostEnvironment hostingEnvironment)
        {
            _userRepository = userRepository;
            _userContext =  userContext;
            _hostingEnvironment = hostingEnvironment;
        }

        public List<UserDocument> GetAll() =>
            _userRepository.Get();

        public UserDocument Get(Guid id) =>
            _userRepository.Get(id);

        public UserDocument GetByEmail(string email) =>
            _userRepository.Get(email);

        public UserDocument Create(UserDocument userDocument)
        {
            var user = GetByEmail(userDocument.Email);

            if (user != null)
            {
                throw new UnauthorizedAccessException("User already Exists");
            }

            return _userRepository.Create(userDocument);
        }

        public void Update(Guid id, UserInfo userDocument)
        {
            var user = Get(userDocument.Id);

            if (user != null)
            {
                user.FirstName = userDocument.FirstName;
                user.LastName = userDocument.LastName;
                user.PassportPhoto = userDocument.PassportPhoto;
                user.ValidIdPhoto = userDocument.ValidIdPhoto;
                if (user.BVN == null || user.BVN.Length == 0)
                {
                    user.BVN = userDocument.BVN;
                }
                if (user.PhoneNumber == null || user.PhoneNumber.Length == 0)
                {
                    user.PhoneNumber = userDocument.PhoneNumber;
                }
                if (user.Email == null || user.Email.Length == 0)
                {
                    user.Email = userDocument.Email;
                }

                _userRepository.Update(id, user);
            }
        }

        public void UpdateFile(Guid Id, IFormFile file)
        {
            var user = Get(Id);

            var allowedContentTypes = new string[] { "image/jpg", "image/png", "image/jpeg", "application/pdf" };
            try
            {
                Guid username = _userContext.UserId.Value;
                if (!allowedContentTypes.Contains(file.ContentType))
                {
                    throw new InvalidOperationException("Invalid image extension");
                }
                string folderName = "Upload";
                string webRootPath = _hostingEnvironment.ContentRootPath;
                string newPath = "";
                if (file.Name == "idCard")
                {
                    newPath = Path.Combine(webRootPath, folderName, "Users", "IdCard", $"{Id}");
                }
                if (file.Name == "passportPhoto")
                {
                    newPath = Path.Combine(webRootPath, folderName, "Users", "PassportPhoto", $"{Id}");
                }

                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    if (file.Name == "idCard")
                    {
                        user.ValidIdPhoto = fileName;
                    }

                    if (file.Name == "passportPhoto")
                    {
                        user.PassportPhoto = fileName;
                    }
                }

                _userRepository.Update(Id, user);

            }
            catch (System.Exception ex)
            {
                throw new Exception("Upload Failed, " + ex.Message);
            }

        }

        public void Remove(Guid id) =>
            _userRepository.Remove(id);

    }
}
