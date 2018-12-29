using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTools.Authentication.Interfaces.AuthenticationTokens
{
    public interface IGoogleAuthenticationTokens
    {
        string AccessToken { get; set; }
        string RefreshToken { get; set; }
        JObject IdToken { get; set; }
    }
}
