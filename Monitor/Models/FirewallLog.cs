using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.Models
{
    public class FirewallLog
    {
        DateTime Date { get; set; }

        string AttarkerIp { get; set; }

        string TargetedRouterIpAndPort { get; set; }

        string Country { get; set; }

        string City { get; set; }

        string Latitude { get; set; }

        string Longitude { get; set; }
    }
}
