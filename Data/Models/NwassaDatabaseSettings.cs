using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Nwassa.Data.Models.NwassaDatabaseSettings;

namespace Nwassa.Data.Models
{
    public class NwassaDatabaseSettings : INwassaDatabaseSettings
    {

        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

        public interface INwassaDatabaseSettings
        {
            string ConnectionString { get; set; }
            string DatabaseName { get; set; }
        }
    }
}
