using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlackFolderGames.Web.Models
{
    public class EditClaimsModel
    {
        public EditClaimsModel() { }
        public EditClaimsModel(UserModel model)
        {
            User = model.IdentityUser;
            Claims = model.Claims.Select(c => new EditClaimsModel.Claim() { Type = c.Type, Value = c.Value }).ToArray();
        }
        public IdentityUser User { get; set; }
        public EditClaimsModel.Claim[] Claims { get; set; }
        public class Claim
        {
            public string Type { get; set; }
            public string Value { get; set; }
        }
    }
}
