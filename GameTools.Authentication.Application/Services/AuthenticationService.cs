using GameTools.Authentication.Interfaces.AuthenticationProviders;
using GameTools.Authentication.Interfaces.Repositories;
using GameTools.Authentication.Interfaces.Services;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GameTools.Authentication.Data.Repositories;
using GameTools.Authentication.Interfaces.Enums;
using System.Security.Principal;
using System.Security.Claims;
using System.Linq;
using GameTools.Authentication.Interfaces.DataModels;
using GameTools.Authentication.Interfaces.Entities;

namespace GameTools.Authentication.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private IAuthenticationRepository _repo;
        private IAuthenticationProviderFactory _authProviderFactory;
        private IConfiguration _config;
        public AuthenticationService(IConfiguration config, IAuthenticationRepositoryFactory authRepoFactory, IAuthenticationProviderFactory authProviderFactory)
        {
            _repo = authRepoFactory.CreateRepository(config.GetConnectionString("GameTools:Authentication"));
            _config = config;
            _authProviderFactory = authProviderFactory;
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IAuthenticationRepositoryFactory, AuthenticationRepositoryFactory>();
        }

        public IPrincipal Authenticate(ThirdPartyAuthenticationProvider provider, string userIdentifier)
        {
            var keys = GetThirdPartyValidationKeys(provider);
            IAuthenticationProvider authProvider = _authProviderFactory.GetAuthenticationProvider(provider, keys);
            ClaimsIdentity thirdPartyIdentity = authProvider.Authenticate(userIdentifier) as ClaimsIdentity;
            if (thirdPartyIdentity != null && thirdPartyIdentity.HasClaim(c => c.Type == "GameToolsKey")) {
                var principal = new ClaimsPrincipal(thirdPartyIdentity);
                IUser user = FindUser(provider, thirdPartyIdentity.FindFirst("GameToolKey").Value);
                if (user != null)
                {
                    ClaimsIdentity gameToolsIdentity = CreateClaimsIdentity(user);
                    principal.AddIdentity(gameToolsIdentity);
                }
                return principal;
            }
            throw new ArgumentException("The user identifier provided could not be authenticated.");
        }

        public Dictionary<string,string> GetThirdPartyValidationKeys(ThirdPartyAuthenticationProvider provider)
        {
            string providerName = GetProviderName(provider);
            if(string.IsNullOrEmpty(providerName) || providerName.Equals("None",StringComparison.CurrentCultureIgnoreCase))
            {
                throw new NotImplementedException("The designated provider has not been configured.");
            }
            var keys = _repo.GetThirdPartyValidationKeys(providerName);
            if(keys.Succeeded && keys.Data != null && keys.Data.Any())
            {
                return keys.Data.ToDictionary(d => d.Name, d => d.Value);
            }
            return null;
        }

        private ClaimsIdentity CreateClaimsIdentity(IUser user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("Active",user.Active ? "Active" : "Inactive",ClaimValueTypes.String,"GameTools"),
                new Claim("DisplayName",user.DisplayName, ClaimValueTypes.String, "GameTools"),
                new Claim("DisplayPhotoURL",user.DisplayPhotoURL,ClaimValueTypes.String,"GameTools"),
                new Claim("EmailAddress",user.EmailAddress,ClaimValueTypes.Email,"GameTools"),
                new Claim("GameToolsId",user.Id.ToString(),ClaimValueTypes.String,"GameTools"),
                new Claim("EmailVerified",user.IsEmailVerified ? "Verified" : "Not Verified", ClaimValueTypes.String, "GameTools"),
            };

            claims.AddRange(user.ThirdPartyAuthenticationTokens.Select(tpat => new Claim(tpat.Provider + ":" + tpat.Key, tpat.Token, ClaimValueTypes.String, tpat.Provider)));
            claims.AddRange(user.Claims.Select(c => new Claim(c.Name, c.Value, ClaimValueTypes.String, "GameTools")));
            return new ClaimsIdentity(claims,"GameTools");
        }

        private IUser FindUser(ThirdPartyAuthenticationProvider provider, string key)
        {
            ITransactionResponse<IUser> response = null;
            switch (provider)
            {
                case ThirdPartyAuthenticationProvider.Google:
                    response = _repo.GetUserByGoolgeId(key);
                    break;
            }
            if (response != null && response.Succeeded && response.Data != null)
            {
                return response.Data;              
            }
            return null;
        }

        private string GetProviderName(ThirdPartyAuthenticationProvider provider)
        {
            switch (provider)
            {
                case ThirdPartyAuthenticationProvider.Google:
                    return "Google";
                default:
                    return "None";
            }
        }

        
    }
}
