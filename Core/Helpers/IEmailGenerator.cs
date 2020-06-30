using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Nwassa.Core.Helpers.EmailGenerator;

namespace Nwassa.Core.Helpers
{
    public interface IEmailGenerator
    {
        string SendMail(EmailModel model);
    }
}
