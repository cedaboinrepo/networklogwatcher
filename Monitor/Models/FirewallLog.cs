using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.Models
{
    public class FirewallLog
    {
        public DateTime Date { get; set; }

        public string AttarkerIp { get; set; }

        public string TargetedRouterIpAndPort { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }
    }
}
