using GameTools.Authentication.Interfaces.AuthenticationProviders;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using GameTools.Authentication.Interfaces.Enums;
using System.Security.Principal;

namespace GameTools.Authentication.Interfaces.Services
{
    public interface IAuthenticationService
    {
        IPrincipal Authenticate(ThirdPartyAuthenticationProvider provider, string userIdentifier);
        Dictionary<string, string> GetThirdPartyValidationKeys(ThirdPartyAuthenticationProvider provider);
    }
}
