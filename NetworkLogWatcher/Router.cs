using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetworkLogWatcher
{
    public static class Router
    {
        public static string Get()
        {
            var data = Ping();

            return data;
        }

        public static string Ping()
        {
            string result = string.Empty;
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("http://10.0.0.1");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = client.GetAsync("/").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        result = response.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception ex)
                {
                    var error = ex;
                }

                return result;
            }
        }

        public static void Login()
        {
            // http://10.0.0.1/goform/home_loggedout
            string result = string.Empty;
            using (var client = new HttpClient())
            {
                try
                {
                    var values = new Dictionary<string, string>();
                    values.Add("loginUsername", "admin");
                    values.Add("loginPassword", "DgKvjnm5CAkLS16nGVfZ");
                    var content = new FormUrlEncodedContent(values);

                    client.BaseAddress = new Uri("http://10.0.0.1");
                    client.DefaultRequestHeaders.Accept.Clear();

                    HttpResponseMessage response = client.PostAsync("goform/home_loggedout", content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        result = response.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception ex)
                {
                    var error = ex;
                }

            }
        }

        public static FirewallLog GetFirewallLogs()
        {
            // http://10.0.0.1/goform/home_loggedout
            string result = string.Empty;
            FirewallLog firewallLog = null;

            using (var client = new HttpClient())
            {
                try
                {
                    var values = new Dictionary<string, string>();
                    values.Add("logtype", "2");
                    values.Add("timeframe", "0");
                    values.Add("ShowLogs", "Show Logs");
                    var content = new FormUrlEncodedContent(values);

                    client.BaseAddress = new Uri("http://10.0.0.1");
                    client.DefaultRequestHeaders.Accept.Clear();

                    HttpResponseMessage response = client.PostAsync("goform/troubleshooting_logs", content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var htmlPage = response.Content.ReadAsByteArrayAsync().Result;

                        String source = Encoding.GetEncoding("utf-8").GetString(htmlPage, 0, htmlPage.Length - 1);
                        source = WebUtility.HtmlDecode(source);
                        HtmlDocument resultat = new HtmlDocument();
                        resultat.LoadHtml(source);

                        var tables = resultat.DocumentNode.SelectNodes(".//table");
                        var row = tables[0].ChildNodes["tbody"].InnerText;
                        row = row.Replace("\n", "");
                        row = row.Replace("\t", "");
                        var tokens = row.Split(' ');

                        var logs = new List<string>();

                        foreach (var token in tokens)
                        {
                            if (token.Length > 2)
                            {
                                logs.Add(token);
                            }
                        }

                        var key_1 = Regex.Replace(logs[5], @"\s+", " ").Split(' ');
                        var key_2 = Regex.Replace(logs[8], @"\s+", " ");
                        var values_2 = key_2.Split(' ');

                        var time = logs[7];
                        var dayOfWeek = key_1[2];
                        var day = DateTime.Now.Day;
                        var month = logs[6];
                        var year = values_2[0];

                        var textDate = dayOfWeek + " " + day + " " + month + " " + year + " " + time;

                        firewallLog = new FirewallLog
                        {
                            Date = Convert.ToDateTime(day + " " + month + " " + year + " " + time),
                            Destination = values_2[1],
                            Source = values_2[2],
                        };
                    }
                }
                catch (Exception ex)
                {
                    var error = ex;
                }
                return firewallLog;
            }
        }
    }
}
