using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace GameTools.Authentication.Interfaces.AuthenticationProviders
{
    public interface IAuthenticationProvider
    {
        IIdentity Authenticate(string userIdentifier);
    }
}
