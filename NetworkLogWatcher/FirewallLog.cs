using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkLogWatcher
{
    public class FirewallLog
    {
        public DateTime Date { get; set; }

        public string Source { get; set; }

        public string Destination { get; set; }
    }
}
