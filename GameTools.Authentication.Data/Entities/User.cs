using GameTools.Authentication.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace GameTools.Authentication.Data.Entities
{
    public class User : IUser
    {
        private IEnumerable<IClaim> _claims;
        private IEnumerable<IThirdPartyAuthenticationToken> _tokens;

        public string DisplayName { get; set; }
        public string DisplayPhotoURL { get; set; }
        public string EmailAddress { get; set; }
        IEnumerable<IClaim> IUser.Claims { get { return _claims; } set { _claims = value; } }
        public List<Claim> Claims { get { return _claims as List<Claim>; } set { _claims = value; } }
        public Guid Id { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool Active { get; set; }
        IEnumerable<IThirdPartyAuthenticationToken> IUser.ThirdPartyAuthenticationTokens { get { return _tokens; } set { _tokens = value; } }
        public List<ThirdPartyAuthenticationToken> ThirdPartyAuthenticationTokens { get { return _tokens as List<ThirdPartyAuthenticationToken>; } set { _tokens = value; } }

    }
}
