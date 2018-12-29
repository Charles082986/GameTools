using GameTools.Authentication.Interfaces.AuthenticationTokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTools.Authentication.Interfaces.AuthenticationProviders
{
    public interface IGoogleAuthenticationProvider : IAuthenticationProvider
    {
        string ClientId { get; set; }
        string ClientSecret { get; set; }
        string ApplicationName { get; set; }
        string[] Scopes { get; set; }
    }
}
