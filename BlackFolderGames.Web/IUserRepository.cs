using System.Collections.Generic;
using System.Security.Claims;
using BlackFolderGames.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace BlackFolderGames.Web
{
    public interface IUserRepository
    {
        UserManager<IdentityUser> Manager { get; set; }

        bool AddOrUpdateClaim(IdentityUser user, IList<Claim> claims, string type, string value, string issuer = "BFG");
        void BanUser(IdentityUser user, string reason = "");
        IEnumerable<UserModel> GetUsers();
        UserModel GetUser(string userId);
        bool RemoveClaimIfExists(IdentityUser user, IList<Claim> claims, string type, string issuer = "BFG");
        void SuspendUser(IdentityUser user, string reason = "", int days = 30);
        void UnbanUser(IdentityUser user);
        void UnsuspendUser(IdentityUser user);
    }
}