using BlackFolderGames.Web.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlackFolderGames.Web
{


    public class UserRepository : IUserRepository
    {

        public UserRepository(UserManager<IdentityUser> userManager)
        {
            Manager = userManager;
        }

        public UserManager<IdentityUser> Manager { get; set; }

        public UserModel GetUser(string userId)
        {
            var identity = Manager.FindByIdAsync(userId).Result;
            return new UserModel(identity, Manager.GetClaimsAsync(identity).Result.ToList());
        }

        public IEnumerable<UserModel> GetUsers()
        {
            var users = Manager.Users.ToList();
            return users.Select(u =>
            {
                var c = Manager.GetClaimsAsync(u).Result.ToList();
                return new UserModel(u, c);
            }).ToList();
        }

        public void UnsuspendUser(IdentityUser user)
        {
            IList<Claim> claims = Manager.GetClaimsAsync(user).Result;
            RemoveClaimIfExists(user, claims, "SUSPENSION_END");
            RemoveClaimIfExists(user, claims, "SUSPENSION");
        }

        public void SuspendUser(IdentityUser user, string reason = "", int days = 30)
        {
            IList<Claim> claims = Manager.GetClaimsAsync(user).Result;
            AddOrUpdateClaim(user, claims, "SUSPENSION_END", DateTime.Now.AddDays(days).ToString("YYYY-MM-DD"));
            AddOrUpdateClaim(user, claims, "SUSPENSION", reason);
        }

        public void BanUser(IdentityUser user, string reason = "")
        {
            IList<Claim> claims = Manager.GetClaimsAsync(user).Result;
            AddOrUpdateClaim(user, claims, "BANNED", reason);
        }

        public void UnbanUser(IdentityUser user)
        {
            IList<Claim> claims = Manager.GetClaimsAsync(user).Result;
            RemoveClaimIfExists(user, claims, "BANNED");
        }

        public bool AddOrUpdateClaim(IdentityUser user, IList<Claim> claims, string type, string value, string issuer = "BFG")
        {
            Claim claim = claims.FirstOrDefault(c => c.Type == type && c.Issuer == issuer);
            if(claim == null)
            {
                return Manager.AddClaimAsync(user, new Claim(type, value, ClaimValueTypes.String, issuer)).Result.Succeeded;
            }
            else
            {
                return Manager.RemoveClaimAsync(user, claim).Result.Succeeded && Manager.AddClaimAsync(user, new Claim(type, value, ClaimValueTypes.String, issuer)).Result.Succeeded;
            }
        }

        public bool RemoveClaimIfExists(IdentityUser user, IList<Claim> claims, string type, string issuer = "BFG")
        {
            Claim claim = claims.FirstOrDefault(c => c.Type == type && c.Issuer == issuer);
            if (claim != null)
            {
                return Manager.RemoveClaimAsync(user, claim).Result.Succeeded;
            }
            else
            {
                return false;
            }
        }
    }
}
