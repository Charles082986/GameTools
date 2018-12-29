using GameTools.Authentication.Application.AuthenticationTokens;
using GameTools.Authentication.Interfaces.AuthenticationProviders;
using GameTools.Authentication.Interfaces.AuthenticationTokens;
using GameTools.Authentication.Interfaces.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace GameTools.Authentication.Application.AuthenticationProviders
{
    public class GoogleAuthenticationProvider : IGoogleAuthenticationProvider
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ApplicationName { get; set; }
        public string[] Scopes { get; set; }

        public GoogleAuthenticationProvider(string clientId, string clientSecret, string applicationName, string[] scopes)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            ApplicationName = applicationName;
            Scopes = scopes;
        }

        public IIdentity Authenticate(string userIdentifier)
        {
            try
            {
                GoogleAuthenticationTokens tokens = new GoogleAuthenticationTokens();
                UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets { ClientId = ClientId, ClientSecret = ClientSecret },
                        Scopes,
                        userIdentifier,
                        CancellationToken.None)
                    .Result;
                return GetClaimsIdentity(credential);
            }
            catch(Exception ex)
            {
                log.Error("Google Authentication Provider - User Identifier: " + userIdentifier, ex);
                return null;
            }
        }

        private ClaimsIdentity GetClaimsIdentity(UserCredential credential)
        {
            var claims = new List<Claim>() {
                new Claim("UserId", credential.UserId, ClaimValueTypes.String, "Google"),
                new Claim("AccessToken", credential.Token.AccessToken, ClaimValueTypes.String, "Google"),
                new Claim("RefreshToken", credential.Token.RefreshToken, ClaimValueTypes.String, "Google"),
                new Claim("Scope", credential.Token.Scope, ClaimValueTypes.String, "Google")
            };
            
            JObject idToken = JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(Convert.FromBase64String(credential.Token.IdToken)));
            foreach(JProperty property in idToken.Properties())
            {
                claims.Add(new Claim(property.Name, property.Value.ToString(), ClaimValueTypes.String, "Google"));
            }

            claims.Add(new Claim("GameToolsKey", idToken["sub"].ToString(), ClaimValueTypes.String, "Google"));

            return new ClaimsIdentity(claims, "Google");
        }


    }
}
