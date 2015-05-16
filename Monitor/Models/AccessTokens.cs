using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.Models
{
    public class AccessTokens
    {
        public string token_type;

        public int expires_in { get; set; }

        public string scope { get; set; }

        public string refresh_token = null;

        public string access_token { get; set; }

        public string user_id { get; set; }
    }
}