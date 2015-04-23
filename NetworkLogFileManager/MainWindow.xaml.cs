using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Net;
using System.Web.Script.Serialization;
using MicrosoftAccount.WindowsForms;
using System.Threading.Tasks;

namespace NetworkLogFileManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string MsaClientId = "000000004814F681";
        private const string MsaClientSecret = "cnNgx5df5xoL9IK9Um2aZLZ7BGDCWEHI";
        private static AppTokenResult _appToken = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private static async Task Authenticate()
        {
            var oldRefreshToken = string.Empty;

            if (!string.IsNullOrEmpty(oldRefreshToken))
            {
                _appToken = await MicrosoftAccountOAuth.RedeemRefreshTokenAsync(MsaClientId, MsaClientSecret, oldRefreshToken);
            }

            if (null == _appToken)
            {
                _appToken = await MicrosoftAccountOAuth.LoginAuthorizationCodeFlowAsync(MsaClientId,
                    MsaClientSecret,
                    new[] { "wl.offline_access", "wl.basic", "wl.signin", "onedrive.readwrite" });
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //await Authenticate();

            OAuthForm form = new OAuthForm();
            form.Start();

            // create folder

            // upload file

            // download file
        }
    }
}

