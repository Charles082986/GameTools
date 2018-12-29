using System;
using System.Collections.Generic;
using System.Text;

namespace GameTools.Authentication.Interfaces.Entities
{
    public interface IClaim : IUniqueBase<Guid>
    {
        IUser User { get; set; }
        string Name { get; set; }
        string Value { get; set; }
        string Provider { get; set; }
    }
}
