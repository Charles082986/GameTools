using GameTools.Authentication.Interfaces.AuthenticationProviders;
using GameTools.Authentication.Interfaces.Enums;
using GameTools.Authentication.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTools.Authentication.Application.AuthenticationProviders
{
    public class AuthenticationProviderFactory : IAuthenticationProviderFactory
    {
        private IConfiguration _config;
        private IAuthenticationService _auth;
        public AuthenticationProviderFactory(IConfiguration config, IAuthenticationService auth)
        {
            _config = config;
            _auth = auth;
        }

        public IAuthenticationProvider GetAuthenticationProvider(ThirdPartyAuthenticationProvider provider, Dictionary<string,string> keys)
        {
            switch(provider)
            {
                case ThirdPartyAuthenticationProvider.Google:
                    string clientId = keys["ClientId"];
                    string clientSecret = keys["ClientSecret"];
                    string applicationName = keys["ApplicationName"];
                    string[] scopes = (keys["Scopes"] ?? "").Split('|');
                    return GetGoogleAuthenticationProvider(clientId, clientSecret, applicationName, scopes);
            }
            return null;
        }

        public IGoogleAuthenticationProvider GetGoogleAuthenticationProvider(string clientId, string clientSecret, string applicationName, string[] scopes)
        {
            return new GoogleAuthenticationProvider(clientId, clientSecret, applicationName, scopes);
        }
    }
}
