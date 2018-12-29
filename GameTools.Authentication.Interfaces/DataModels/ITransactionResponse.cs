using GameTools.Authentication.Interfaces.Enums;
using System;
using System.Collections.Generic;
using System.Text;


namespace GameTools.Authentication.Interfaces.DataModels
{
    public interface ITransactionResponse<T>
    {
        TransactionStatus Status { get; set; }
        bool Succeeded { get; }
        Exception Exception { get; set; }
        T Data { get; set; }
    }
}
