using System;
using System.Collections.Generic;
using System.Text;

namespace GameTools.Authentication.Interfaces.Enums
{
    public enum TransactionStatus
    {
        Success = 0,
        UnknownError = 1,
        UserNotFound = 2,
        RoleNotFound = 3
    }
}
