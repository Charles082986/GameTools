using System;
using System.Collections.Generic;
using System.Text;

namespace GameTools.Authentication.Interfaces.Entities
{
    public interface IThirdPartyValidationKey
    {
        string Provider { get; set; }
        string Name { get; set; }
        string Value { get; set; }
    }
}
