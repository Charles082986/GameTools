using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlackFolderGames.Web.Models
{
    public class UserModel
    {
        public UserModel() { }
        public UserModel(IdentityUser user, List<Claim> claims)
        {
            IdentityUser = user;
            Claims = claims;
        }

        public IdentityUser IdentityUser { get; set; }
        public List<Claim> Claims { get; set; }
    }
}
