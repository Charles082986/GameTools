using System;
using System.Collections.Generic;
using System.Text;

namespace GameTools.Authentication.Interfaces.Entities
{
    public interface IUniqueBase<T>
    {
        T Id { get; set; }
    }
}
