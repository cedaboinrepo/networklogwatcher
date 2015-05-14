using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RouterLogMonitor
{
    internal static class GeoLocation
    {
        public static IpGeoLocation GetIpGeoLocation(FirewallLog log)
        {
            IpGeoLocation result = null;
            using (var client = new HttpClient())
            {
                var index = log.Source.IndexOf(':');
                var port = log.Source.Substring(index);
                log.Source = log.Source.Replace(port, "").Trim();
                try
                {
                    var endpoint = "http://freegeoip.net/json/" + log.Source;
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, endpoint);
                    var response = client.SendAsync(requestMessage).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var textResult = response.Content.ReadAsStringAsync().Result;
                        result = JsonConvert.DeserializeObject<IpGeoLocation>(response.Content.ReadAsStringAsync().Result);
                    }
                }
                catch (Exception ex)
                {
                    var error = ex;
                }
                return result;
            }
        }

        public static IpGeoLocationInfo GetIpGeoLocationInfo(FirewallLog log)
        {
            IpGeoLocationInfo result = null;
            using (var client = new HttpClient())
            {
                var index = log.Source.IndexOf(':');
                if (index != -1)
                {
                    var port = log.Source.Substring(index);
                    log.Source = log.Source.Replace(port, "").Trim();
                }
                try
                {
                    var endpoint = "http://ipinfo.io/" + log.Source;
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, endpoint);
                    var response = client.SendAsync(requestMessage).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var textResult = response.Content.ReadAsStringAsync().Result;
                        result = JsonConvert.DeserializeObject<IpGeoLocationInfo>(response.Content.ReadAsStringAsync().Result);
                    }
                }
                catch (Exception ex)
                {
                    var error = ex;
                }
                return result;
            }
        }
    }

    public class IpGeoLocation
    {
        public string ip { get; set; }
        public string country_code { get; set; }
        public string country_name { get; set; }
        public string region_code { get; set; }
        public string region_name { get; set; }
        public string city { get; set; }
        public string zip_code { get; set; }
        public string time_zone { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string metro_code { get; set; }
    }

    public class IpGeoLocationInfo
    {
        public string ip { get; set; }
        public string hostname { get; set; }
        public string country { get; set; }
        public string region{ get; set; }
        public string city { get; set; }
        public string loc { get; set; }
        public string org { get; set; }
        public string postal { get; set; }
    }
}


