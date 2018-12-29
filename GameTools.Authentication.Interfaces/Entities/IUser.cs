using System;
using System.Collections.Generic;
using System.Text;

namespace GameTools.Authentication.Interfaces.Entities
{
    public interface IUser : IUniqueBase<Guid>
    {
        string DisplayName { get; set; }
        string DisplayPhotoURL { get; set; }
        string EmailAddress { get; set; }
        bool Active { get; set; }
        bool IsEmailVerified { get; set; }
        IEnumerable<IThirdPartyAuthenticationToken> ThirdPartyAuthenticationTokens { get; set; }
        IEnumerable<IClaim> Claims { get; set; }
    }
}
