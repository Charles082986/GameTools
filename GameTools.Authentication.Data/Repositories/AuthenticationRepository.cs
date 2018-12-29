using GameTools.Authentication.Data.DataModels;
using GameTools.Authentication.Data.Entities;
using GameTools.Authentication.Interfaces.DataModels;
using GameTools.Authentication.Interfaces.Entities;
using GameTools.Authentication.Interfaces.Enums;
using GameTools.Authentication.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameTools.Authentication.Data.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Security _security;
        private AuthenticationDbContext _ctx;
        public AuthenticationRepository(string connectionString, Security security)
        {
            _security = security;
            _ctx = new AuthenticationDbContextFactory().CreateDbContext(connectionString);
        }

        public ITransactionResponse<IClaim> CreateClaim(IClaim claim)
        {
            using (var transaction = _ctx.Database.BeginTransaction())
            {
                try
                {
                    var item = _ctx.Claims.Add(claim as Claim);
                    _ctx.SaveChanges();
                    transaction.Commit();
                    return new TransactionResponse<IClaim>(_ctx.Claims.Find(item.Entity.Id),null);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    log.Error("Authentication Repository Error", ex);
                    return new TransactionResponse<IClaim>(null, ex);
                }
            }
        }

        public ITransactionResponse<IUser> CreateUser(IUser user)
        {
            using(var transaction = _ctx.Database.BeginTransaction())
            {
                try
                {
                    var item = _ctx.Users.Add(user as User);
                    _ctx.SaveChanges();
                    transaction.Commit();
                    return new TransactionResponse<IUser>(_ctx.Users
                        .Include(u => u.Claims)
                        .Include(u => u.ThirdPartyAuthenticationTokens)
                        .FirstOrDefault(u => u.Id == item.Entity.Id), null);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    log.Error("Authentication Repository Error", ex);
                    return new TransactionResponse<IUser>(null, ex);
                }
            }
        }

        public ITransactionResponse<IClaim> DeleteClaim(IClaim claim)
        {
            using (var transaction = _ctx.Database.BeginTransaction())
            {
                try
                {
                    var item = _ctx.Claims.Remove(claim as Claim);
                    _ctx.SaveChanges();
                    transaction.Commit();
                    return new TransactionResponse<IClaim>(_ctx.Claims.Find(item.Entity.Id), null);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    log.Error("Authentication Repository Error", ex);
                    return new TransactionResponse<IClaim>(null, ex);
                }
            }
        }

        public ITransactionResponse<IUser> DeleteUser(IUser user)
        {
            using (var transaction = _ctx.Database.BeginTransaction())
            {
                try
                {
                    user.Active = false;
                    var item = _ctx.Users.Update(user as User);
                    _ctx.SaveChanges();
                    transaction.Commit();
                    return new TransactionResponse<IUser>(_ctx.Users.Find(item.Entity.Id), null);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    log.Error("Authentication Repository Error", ex);
                    return new TransactionResponse<IUser>(null, ex);
                }
            }
        }

        public ITransactionResponse<IClaim> GetClaim(int id)
        {
            try
            {
                return new TransactionResponse<IClaim>(_ctx.Claims.Find(id), null);
            }
            catch(Exception ex)
            {
                log.Error("Authentication Repository Error", ex);
                return new TransactionResponse<IClaim>(null, ex);
            }
        }

        public ITransactionResponse<IEnumerable<IClaim>> GetClaims()
        {
            try
            {
                return new TransactionResponse<IEnumerable<IClaim>>(_ctx.Claims, null);
            }
            catch(Exception ex)
            {
                log.Error("Authentication Repository Error", ex);
                return new TransactionResponse<IEnumerable<IClaim>>(null, ex);
            }
        }

        public ITransactionResponse<IEnumerable<IClaim>> GetClaimsByUser(string email)
        {
            try
            {
                User user = _ctx.Users.Include(u => u.Claims).FirstOrDefault(u => u.EmailAddress == email && u.IsEmailVerified);
                if(user == null)
                {
                    return new TransactionResponse<IEnumerable<IClaim>>(TransactionStatus.UserNotFound, null, null);
                }
                else
                {
                    return new TransactionResponse<IEnumerable<IClaim>>(_ctx.Claims.Where(r => user.Claims.Contains(r)), null);
                }
            }
            catch (Exception ex)
            {
                return new TransactionResponse<IEnumerable<IClaim>>(null, ex);
            }
        }

        public ITransactionResponse<IEnumerable<IThirdPartyValidationKey>> GetThirdPartyValidationKeys(string provider)
        {
            try
            {
                return new TransactionResponse<IEnumerable<IThirdPartyValidationKey>>(_ctx.ThirdPartyValidationKeys.Where(k => k.Provider == provider).ToList(),null);
            }
            catch (Exception ex)
            {
                return new TransactionResponse<IEnumerable<IThirdPartyValidationKey>>(null, ex);
            }
        }

        public ITransactionResponse<IUser> GetUser(Guid id)
        {
            try
            {
                var user = _ctx.Users.Include(u => u.Claims).FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return new TransactionResponse<IUser>(TransactionStatus.UserNotFound, null, null);
                }
                else
                {
                    return new TransactionResponse<IUser>(user, null);
                }
            }
            catch (Exception ex)
            {
                log.Error("Authentication Repository Error", ex);
                return new TransactionResponse<IUser>(null, ex);
            }
        }

        public ITransactionResponse<IUser> GetUserByEmail(string email)
        {
            try
            {
                var user = _ctx.Users.Include(u => u.Claims).FirstOrDefault(u => u.EmailAddress == email && u.IsEmailVerified);
                if (user == null)
                {
                    return new TransactionResponse<IUser>(TransactionStatus.UserNotFound, null, null);
                }
                else
                {
                    return new TransactionResponse<IUser>(user, null);
                }
            }
            catch (Exception ex)
            {
                log.Error("Authentication Repository Error", ex);
                return new TransactionResponse<IUser>(null, ex);
            }
        }

        public ITransactionResponse<IUser> GetUserByGoolgeId(string sub)
        {
            try
            {
                string token = sub; // Encoding.UTF8.GetString(_security.GetHash(sub));
                return new TransactionResponse<IUser>(
                    _ctx.Users
                        .Include(u => u.Claims)
                        .Include(u => u.ThirdPartyAuthenticationTokens)
                        .FirstOrDefault(u =>
                            u.ThirdPartyAuthenticationTokens.Any(t =>
                                t.Provider == "Google"
                                && t.Key == "sub"
                                && t.Token == token)),null);

            }
            catch(Exception ex)
            {
                return new TransactionResponse<IUser>(null, ex);
            }

        }

        public ITransactionResponse<IEnumerable<IUser>> GetUsers()
        {
            try
            {
                return new TransactionResponse<IEnumerable<IUser>>(_ctx.Users.Include(u => u.Claims), null);
            }
            catch (Exception ex)
            {
                log.Error("Authentication Repository Error", ex);
                return new TransactionResponse<IEnumerable<IUser>>(null, ex);
            }
        }

        public ITransactionResponse<IEnumerable<IUser>> GetUsersWithVerifiedEmails()
        {
            try
            {
                return new TransactionResponse<IEnumerable<IUser>>(_ctx.Users.Include(u => u.Claims).Where(u => u.IsEmailVerified), null);
            }
            catch (Exception ex)
            {
                log.Error("Authentication Repository Error", ex);
                return new TransactionResponse<IEnumerable<IUser>>(null, ex);
            }
        }

        public ITransactionResponse<IClaim> UpdateClaim(IClaim claim)
        {
            using (var transaction = _ctx.Database.BeginTransaction())
            {
                try
                {
                    _ctx.Claims.Update(claim as Claim);
                    _ctx.SaveChanges();
                    transaction.Commit();
                    return new TransactionResponse<IClaim>(_ctx.Claims.Find(claim.Id), null);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    log.Error("Authentication Repository Error", ex);
                    return new TransactionResponse<IClaim>(null, ex);
                }
            }
        }

        public ITransactionResponse<IThirdPartyValidationKey> UpdateThirdPartyValidationKey(IThirdPartyValidationKey thirdPartyValidationKey)
        {
            using (var transaction = _ctx.Database.BeginTransaction())
            {
                try
                {
                    _ctx.ThirdPartyValidationKeys.Update(thirdPartyValidationKey as ThirdPartyValidationKey);
                    _ctx.SaveChanges();
                    transaction.Commit();
                    return new TransactionResponse<IThirdPartyValidationKey>(_ctx.ThirdPartyValidationKeys.Find(thirdPartyValidationKey.Name, thirdPartyValidationKey.Provider), null);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    log.Error("Authentication Repository Error", ex);
                    return new TransactionResponse<IThirdPartyValidationKey>(null, ex);
                }
            }
        }

        public ITransactionResponse<IUser> UpdateUser(IUser user)
        {
            using (var transaction = _ctx.Database.BeginTransaction())
            {
                try
                {
                    _ctx.Users.Update(user as User);
                    _ctx.SaveChanges();
                    transaction.Commit();
                    return new TransactionResponse<IUser>(_ctx.Users.Find(user.Id), null);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    log.Error("Authentication Repository Error", ex);
                    return new TransactionResponse<IUser>(null, ex);
                }
            }
        }
    }
}
