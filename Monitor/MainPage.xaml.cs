using Monitor.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Monitor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static string _code;
        private static string _error;

        private const string ClientId = "000000004814F681";

        // Request-related URIs that you use to get an authorization code, 
        // access token, and refresh token.

        private const string AuthorizeUri = "https://login.live.com/oauth20_authorize.srf";
        private const string TokenUri = "https://login.live.com/oauth20_token.srf";
        private const string DesktopUri = "https://login.live.com/oauth20_desktop.srf";
        private const string RedirectPath = "/oauth20_desktop.srf";
        private const string ConsentUriFormatter = "{0}?client_id={1}&scope=wl.offline_access,wl.basic,wl.signin,onedrive.readwrite&response_type=code&redirect_uri={2}";
        private const string AccessUriFormatter = "{0}?client_id={1}&code={2}&grant_type=authorization_code&redirect_uri={3}";
        private const string RefreshUriFormatter = "{0}?client_id={1}&grant_type=refresh_token&redirect_uri={2}&refresh_token={3}";
        public MainPage()
        {
            this.InitializeComponent();
            WebViewMap.Source = new Uri("ms-appx-web:///Pages/Map.html");
        }

        public void StartAuthentication()
        {
            var uri = string.Format(ConsentUriFormatter, AuthorizeUri, ClientId, DesktopUri);
            WebViewMap.Navigate(new Uri(uri));
        }

        //private async void WebBroserLiveAuthenticate_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        //{
        //    if (e.Uri.AbsolutePath.Equals(RedirectPath))
        //    {
        //        if (!string.IsNullOrEmpty(e.Uri.Query))
        //        {
        //            Dictionary<string, string> parameters = ParseQueryString(e.Uri.Query, new[] { '&', '?' });

        //            if (parameters.ContainsKey("code"))
        //            {
        //                _code = parameters["code"];

        //                var uri = string.Format(AccessUriFormatter, TokenUri, ClientId, _code, DesktopUri);
        //                var tokens = await GetAccessTokens(uri);

        //                MainWindow window = (MainWindow)_mainWindow;
        //                window.AccessToken = tokens.access_token;
        //                window.RefeshToken = tokens.refresh_token;
        //                window.TokenExpires = tokens.expires_in;

        //                window.LabelToken.Text = string.Format("AccessToken: {0}",tokens.access_token);
        //                window.LabelRefreshToken.Text = string.Format("RefreshToken: {0}", tokens.refresh_token);
        //                window.LabellUploadedFile.Text = "?";
        //            }
        //            else
        //            {
        //                _error = Uri.UnescapeDataString(parameters["error_description"]);
        //            }
        //            this.Close();
        //        }
        //    }
        //}

        // Parses the URI query string. The query string contains a list of name-value pairs 
        // following the '?'. Each name-value pair is separated by an '&'.

        private static Dictionary<string, string> ParseQueryString(string query, char[] delimiters)
        {
            var parameters = new Dictionary<string, string>();

            string[] pairs = query.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            foreach (string pair in pairs)
            {
                string[] nameValue = pair.Split(new[] { '=' });
                parameters.Add(nameValue[0], nameValue[1]);
            }

            return parameters;
        }

        private static async Task<AccessTokens> GetAccessTokens(string uri)
        {
            AccessTokens tokenResponse = null;

            try
            {
                var realUri = new Uri(uri, UriKind.Absolute);
                var addy = realUri.AbsoluteUri.Substring(0, realUri.AbsoluteUri.Length - realUri.Query.Length);
                var test = realUri.Query.Substring(1);

                using (var client = new HttpClient())
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, addy + "?" + test);
                    requestMessage.Headers.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                    var response = await client.SendAsync(requestMessage);
                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    var content = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<AccessTokens>(content);
                }
            }
            catch (WebException e)
            {
                var response = (HttpWebResponse)e.Response;
            }
            return tokenResponse;
        }

        //private async Task CreateAppFolder(string accessToken)
        //{
        //    // => /drive/special/approot
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://api.onedrive.com/v1.0/drive/special/approot");
        //            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        //            var response = await client.SendAsync(requestMessage);
        //            if (!response.IsSuccessStatusCode)
        //            {
        //                var error = response;
        //            }

        //            var content = await response.Content.ReadAsStringAsync();
        //        }
        //    }
        //    catch (WebException e)
        //    {
        //        var response = (HttpWebResponse)e.Response;
        //    }
        //}

        private async Task GetAppFolderMeta(string accessToken)
        {
            // => /drive/special/approot
            try
            {
                using (var client = new HttpClient())
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://api.onedrive.com/v1.0/drive/special/approot/children");//"https://api.onedrive.com/v1.0/drive/items");// "6B102E9E8EB0A1A4!115"
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    var response = await client.SendAsync(requestMessage);
                    var status = response.StatusCode;
                    if (!response.IsSuccessStatusCode)
                    {
                        var error = response;
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    dynamic jsonResponse = JsonConvert.DeserializeObject(content);

                    var files = jsonResponse.value;
                    var fileNames = new List<string>();
                    foreach (dynamic o in files)
                    {
                        var obj = o;
                        fileNames.Add((string)o.name);
                    }

                    ComboBoxLogs.ItemsSource = fileNames;
                }
            }
            catch (WebException e)
            {
                var response = (HttpWebResponse)e.Response;
            }
        }

        private async Task DownlaodFile(string accessToken)
        {
            // => /drive/special/approot
            try
            {
                using (var client = new HttpClient())
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://api.onedrive.com/v1.0/drive/items/77C0EF73BDDF0767!20174/content");// "6B102E9E8EB0A1A4!115"
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    var response = await client.SendAsync(requestMessage);
                    var status = response.StatusCode;
                    if (!response.IsSuccessStatusCode)
                    {
                        var error = response;
                    }

                    // header values, need location header to find file content
                    var location = response.RequestMessage.RequestUri.AbsoluteUri;

                    var content = await response.Content.ReadAsStringAsync();

                    var lines = content.Split('\n');
                    var locations = new List<FirewallLog>();
                    foreach (var line in lines)
                    {
                        var values = line.Split('|');
                        if (values.Length != 7)
                            continue;

                        DateTime logDate;

                        locations.Add(
                            new FirewallLog
                            {
                                Date = DateTime.TryParse(values[0], out logDate) ? logDate : DateTime.MinValue,
                                AttarkerIp = string.IsNullOrEmpty(values[1]) ? string.Empty : values[1],
                                TargetedRouterIpAndPort = string.IsNullOrEmpty(values[2]) ? string.Empty : values[2],
                                Country = string.IsNullOrEmpty(values[3]) ? string.Empty : values[3],
                                City = string.IsNullOrEmpty(values[4]) ? string.Empty : values[4],
                                Latitude = string.IsNullOrEmpty(values[5]) ? string.Empty : values[5],
                                Longitude = string.IsNullOrEmpty(values[6]) ? string.Empty : values[6].Replace("\r", ""),
                            });
                    }
                }
            }
            catch (WebException e)
            {
                var response = (HttpWebResponse)e.Response;

            }
        }

        //private async Task UploadFile(string accessToken)
        //{
        //    // => /drive/special/approot
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            var requestMessage = new HttpRequestMessage(HttpMethod.Put, "https://api.onedrive.com/v1.0/drive/items/6B102E9E8EB0A1A4!115/children/test/content");// "6B102E9E8EB0A1A4!115"
        //            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        //            var file = new System.IO.FileStream(@"C:\Users\carlosda\Desktop\page.txt", System.IO.FileMode.Open);
        //            var content = new StreamContent(file);
        //            requestMessage.Content = content;
        //            requestMessage.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("text/plain");


        //            var response = await client.SendAsync(requestMessage);

        //            if (!response.IsSuccessStatusCode)
        //            {
        //                var error = response;
        //            }
        //        }
        //    }
        //    catch (WebException e)
        //    {
        //        var response = (HttpWebResponse)e.Response;

        //    }
        //}

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void WebBroserLiveAuthenticate_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (args.Uri != null && args.Uri.AbsolutePath.Equals(RedirectPath))
            {
                if (!string.IsNullOrEmpty(args.Uri.Query))
                {
                    Dictionary<string, string> parameters = ParseQueryString(args.Uri.Query, new[] { '&', '?' });

                    if (parameters.ContainsKey("code"))
                    {
                        _code = parameters["code"];

                        var uri = string.Format(AccessUriFormatter, TokenUri, ClientId, _code, DesktopUri);
                        var tokens = await GetAccessTokens(uri);

                        await GetAppFolderMeta(tokens.access_token);
                        await DownlaodFile(tokens.access_token);
                        WebViewMap.Source = new Uri("ms-appx-web:///Pages/Map.html");
                    }
                    else
                    {
                        _error = Uri.UnescapeDataString(parameters["error_description"]);
                    }
                    //  this.Close();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartAuthentication();
        }
    }
}
