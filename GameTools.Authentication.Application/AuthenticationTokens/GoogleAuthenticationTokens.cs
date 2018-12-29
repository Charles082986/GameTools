using GameTools.Authentication.Interfaces.AuthenticationTokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTools.Authentication.Application.AuthenticationTokens
{
    public class GoogleAuthenticationTokens : IGoogleAuthenticationTokens
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public JObject IdToken { get; set; }
    }
}
