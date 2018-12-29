using GameTools.Authentication.Interfaces.Enums;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTools.Authentication.Interfaces.AuthenticationProviders
{
    public interface IAuthenticationProviderFactory
    {
        IAuthenticationProvider GetAuthenticationProvider(ThirdPartyAuthenticationProvider provider, Dictionary<string,string> keys);
        IGoogleAuthenticationProvider GetGoogleAuthenticationProvider(string clientId, string clientSecret, string applicationName, string[] scopes);
    }
}
