using System;
using System.Collections.Generic;
using System.Text;

namespace GameTools.Authentication.Interfaces.Entities
{
    public interface IThirdPartyAuthenticationToken : IUniqueBase<Guid>
    {
        IUser User { get; set; }
        string Provider { get; set; }
        string Key { get; set; }
        string Token { get; set; }
    }
}
