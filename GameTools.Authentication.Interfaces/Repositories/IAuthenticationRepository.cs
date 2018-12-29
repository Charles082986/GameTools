using GameTools.Authentication.Interfaces.DataModels;
using GameTools.Authentication.Interfaces.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTools.Authentication.Interfaces.Repositories
{
    public interface IAuthenticationRepository
    {
        ITransactionResponse<IUser> GetUserByEmail(string email);
        ITransactionResponse<IEnumerable<IUser>> GetUsersWithVerifiedEmails();
        ITransactionResponse<IEnumerable<IClaim>> GetClaimsByUser(string email);
        ITransactionResponse<IUser> GetUserByGoolgeId(string id);

        ITransactionResponse<IClaim> CreateClaim(IClaim Claim);
        ITransactionResponse<IClaim> UpdateClaim(IClaim Claim);
        ITransactionResponse<IClaim> GetClaim(int id);
        ITransactionResponse<IEnumerable<IClaim>> GetClaims();
        ITransactionResponse<IClaim> DeleteClaim(IClaim Claim);

        ITransactionResponse<IUser> CreateUser(IUser user);
        ITransactionResponse<IUser> UpdateUser(IUser user);
        ITransactionResponse<IUser> GetUser(Guid id);
        ITransactionResponse<IEnumerable<IUser>> GetUsers();
        ITransactionResponse<IUser> DeleteUser(IUser user);

        ITransactionResponse<IEnumerable<IThirdPartyValidationKey>> GetThirdPartyValidationKeys(string provider);
        ITransactionResponse<IThirdPartyValidationKey> UpdateThirdPartyValidationKey(IThirdPartyValidationKey thirdPartyValidationKey);

    }
}
