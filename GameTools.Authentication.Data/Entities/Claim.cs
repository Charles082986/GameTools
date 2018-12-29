using GameTools.Authentication.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTools.Authentication.Data.Entities
{
    public class Claim : IClaim
    {
        private IUser _user;

        public string Provider { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        IUser IClaim.User { get { return _user; } set { _user = value; } }
        public User User { get { return _user as User; } set { _user = value; } }
        public Guid Id { get; set; }
    }
}
