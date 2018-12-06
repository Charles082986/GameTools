using GameTools.Authentication.Interfaces.DataModels;
using GameTools.Authentication.Interfaces.Enums;
using GameTools.Authentication.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTools.Authentication.Data.DataModels
{
    public class TransactionResponse<T> : ITransactionResponse<T>
    {
        public TransactionResponse(TransactionStatus status, T data, Exception ex)
        {
            Status = status;
            Exception = ex;
            Data = data;
        }

        public TransactionResponse(T data, Exception ex)
        {
            if(ex != null)
            {
                Status = TransactionStatus.UnknownError;
            }
            else
            {
                Status = TransactionStatus.Success;
            }
            Exception = ex;
            Data = data;
        }

        public TransactionStatus Status { get; set; }
        public bool Succeeded { get { return Status == TransactionStatus.Success; } }
        public Exception Exception { get; set; }
        public T Data { get; set; }
    }
}
