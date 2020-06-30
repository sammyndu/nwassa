using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nwassa.Core.Users.Models
{
    public class UserDocument
    {
        public UserDocument()
        {
            Products = new List<Guid>();
        }

        [BsonId]
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string PassportPhoto { get; set; }

        public string ValidIdPhoto { get; set; } 

        public string BVN { get; set; }

        public List<Guid> Products { get; set; }

        public DateTime DateCreated { get; set; }

        public LoginProfile LoginProfile { get; set; }
    }

    public class AddressModel
    {
        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }
    }

    public class LoginProfile
    {
        public string Password { get; set; }

        public string Salt { get; set; }
    }
}
