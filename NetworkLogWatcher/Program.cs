using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkLogWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var data = Router.Get();

            if (!string.IsNullOrEmpty(data))
            {
                Router.Login();

                while (true)
                {
                    var log = Router.GetFirewallLogs();
                    var location = GeoLocation.GetIpGeoLocation(log);

                    if (location != null && !string.IsNullOrEmpty(location.latitude) && !string.IsNullOrEmpty(location.longitude))
                    {
                        var logLine = log.Date + "|" + log.Source + "|" + log.Destination + "|" + location.country_name + "|" + location.city
                            + "|" + location.latitude + "|" + location.longitude;

                        FileOperations.WriteToFile("C:\\Logs\\xfinity_svc1.txt", logLine);
                    }

                    var logInfo = Router.GetFirewallLogs();
                    var locationInfo = GeoLocation.GetIpGeoLocationInfo(log);

                    if (locationInfo != null && !string.IsNullOrEmpty(locationInfo.loc))
                    {
                        var coords = locationInfo.loc.Split(',');
                        var lat = coords[0];
                        var lon = coords[1];

                        var logLineInfo = log.Date + "|" + log.Source + "|" + log.Destination + "|" + locationInfo.country + "|" + locationInfo.city
                            + "|" + lat + "|" + lon;

                        FileOperations.WriteToFile("C:\\Logs\\xfinity_svc2.txt", logLineInfo);
                    }
                }
            }
        }
    }
}