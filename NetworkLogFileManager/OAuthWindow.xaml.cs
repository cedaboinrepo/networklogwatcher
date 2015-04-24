using NetworkLogFileManager.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NetworkLogFileManager
{
    /// <summary>
    /// Interaction logic for OAuthWindow.xaml
    /// </summary>
    public partial class OAuthWindow : Window
    {
        private static WebBrowser _browser;

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
        private Window _mainWindow = null;
        public OAuthWindow()
        {
            InitializeComponent();
        }

        public void StartAuthentication(Window main)
        {
            this.Show();
            _mainWindow = main;

            var uri = string.Format(ConsentUriFormatter, AuthorizeUri, ClientId, DesktopUri);
            WebBroserLiveAuthenticate.Navigate(uri);
        }

        private async void WebBroserLiveAuthenticate_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.Uri.AbsolutePath.Equals(RedirectPath))
            {
                if (!string.IsNullOrEmpty(e.Uri.Query))
                {
                    Dictionary<string, string> parameters = ParseQueryString(e.Uri.Query, new[] { '&', '?' });

                    if (parameters.ContainsKey("code"))
                    {
                        _code = parameters["code"];

                        var uri = string.Format(AccessUriFormatter, TokenUri, ClientId, _code, DesktopUri);
                        var tokens = await GetAccessTokens(uri);

                        MainWindow window = (MainWindow)_mainWindow;
                        window.AccessToken = tokens.access_token;
                        window.RefeshToken = tokens.refresh_token;
                        window.TokenExpires = tokens.expires_in;

                        window.LabelToken.Text = string.Format("AccessToken: {0}",tokens.access_token);
                        window.LabelRefreshToken.Text = string.Format("RefreshToken: {0}", tokens.refresh_token);
                        window.LabellUploadedFile.Text = "?";
                    }
                    else
                    {
                        _error = Uri.UnescapeDataString(parameters["error_description"]);
                    }
                    this.Close();
                }
            }
        }

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
            //var responseSerializer =  JsonConvert.DeserializeObject<IEnumerable<string>>(AccessTokens));
            AccessTokens tokenResponse = null;

            try
            {
                var realUri = new Uri(uri, UriKind.Absolute);

                var addy = realUri.AbsoluteUri.Substring(0, realUri.AbsoluteUri.Length - realUri.Query.Length);

                var test = realUri.Query.Substring(1);
                //var request = (HttpWebRequest)WebRequest.Create(addy);

                //request.Method = "POST";
                //request.ContentType = "application/x-www-form-urlencoded";

                //using (var writer = new StreamWriter(request.GetRequestStream()))
                //{
                //    writer.Write(realUri.Query.Substring(1));
                //}

                //var response = (HttpWebResponse)request.GetResponse();

                // The response is JSON, for example: {
                //                                     "token_type":"bearer",
                //                                     "expires_in":3600,
                //                                     "scope":"bingads.manage",
                //                                     "access_token":"<AccessToken>",
                //                                     "refresh_token":"<RefreshToken>"
                //                                    }

                // Use the JSON serializer to serialize the response into the AccessTokens object.

                //using (Stream responseStream = response.GetResponseStream())
                //{
                //    if (responseStream != null)
                //        tokenResponse = JsonConvert.DeserializeObject<AccessTokens>(responseStream);
                //}

                using (var client = new HttpClient())
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, addy + "?" + test);
                    requestMessage.Headers.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                    //var formValues = new List<KeyValuePair<string, string>>
                    //{
                    //    new KeyValuePair<string, string>("strEmail", email),
                    //    new KeyValuePair<string, string>("ApiKey", apiKey),
                    //    new KeyValuePair<string, string>("strOrgId", orgId)
                    //};
                    //requestMessage.Content = new FormUrlEncodedContent(formValues);

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

                Console.WriteLine("HTTP status code: " + response.StatusCode);
            }

            return tokenResponse;
        }
    }
}

