using GameTools.Authentication.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTools.Authentication.Data.Entities
{
    public class ThirdPartyValidationKey : IThirdPartyValidationKey
    {
        public string Provider { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
