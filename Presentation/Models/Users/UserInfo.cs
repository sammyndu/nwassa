using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nwassa.Presentation.Models.Users
{
    public class UserInfo
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string PassportPhoto { get; set; }

        public string ValidIdPhoto { get; set; }

        public string BVN { get; set; }

        public List<Guid> Products { get; set; }

        public DateTime DateCreated { get; set; }

    }
}
