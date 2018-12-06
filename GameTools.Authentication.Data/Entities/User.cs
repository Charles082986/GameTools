using GameTools.Authentication.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTools.Authentication.Data.Entities
{
    public class User : IUser
    {
        private IEnumerable<IRole> _roles;

        public string DisplayName { get; set; }
        public string DisplayPhotoURL { get; set; }
        public string EmailAddress { get; set; }
        IEnumerable<IRole> IUser.Roles { get { return _roles; } set { _roles = value; } }
        public List<Role> Roles { get { return _roles as List<Role>; } set { _roles = value; } }
        public Guid Id { get; set; }
        public bool IsEmailVerified { get; set; }
        bool Active { get; set; }
    }
}
