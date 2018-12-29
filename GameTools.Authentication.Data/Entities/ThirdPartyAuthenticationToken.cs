using GameTools.Authentication.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GameTools.Authentication.Data.Entities
{
    public class ThirdPartyAuthenticationToken : IThirdPartyAuthenticationToken
    {
        private IUser _user;

        IUser IThirdPartyAuthenticationToken.User { get { return _user; } set { _user = value; } }
        public User User { get { return _user as User; } set { _user = value; } }
        public string Provider { get; set; }
        public string Key { get; set; }
        public string Token { get; set; }
        public Guid Id { get; set; }
    }
}
