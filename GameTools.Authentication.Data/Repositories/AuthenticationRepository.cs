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
        private AuthenticationDbContext _ctx;
        public AuthenticationRepository(string connectionString)
        {
            _ctx = new AuthenticationDbContextFactory().CreateDbContext(connectionString);
        }

        public ITransactionResponse<IRole> CreateRole(IRole role)
        {
            using (var transaction = _ctx.Database.BeginTransaction())
            {
                try
                {
                    var item = _ctx.Roles.Add(role as Role);
                    _ctx.SaveChanges();
                    transaction.Commit();
                    return new TransactionResponse<IRole>(_ctx.Roles.Find(item.Entity.Id),null);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    log.Error("Authentication Repository Error", ex);
                    return new TransactionResponse<IRole>(null, ex);
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

        public ITransactionResponse<IRole> DeleteRole(IRole role)
        {
            using (var transaction = _ctx.Database.BeginTransaction())
            {
                try
                {
                    var item = _ctx.Roles.Remove(role as Role);
                    _ctx.SaveChanges();
                    transaction.Commit();
                    return new TransactionResponse<IRole>(_ctx.Roles.Find(item.Entity.Id), null);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    log.Error("Authentication Repository Error", ex);
                    return new TransactionResponse<IRole>(null, ex);
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

        public ITransactionResponse<IRole> GetRole(int id)
        {
            try
            {
                return new TransactionResponse<IRole>(_ctx.Roles.Find(id), null);
            }
            catch(Exception ex)
            {
                log.Error("Authentication Repository Error", ex);
                return new TransactionResponse<IRole>(null, ex);
            }
        }

        public ITransactionResponse<IEnumerable<IRole>> GetRoles()
        {
            try
            {
                return new TransactionResponse<IEnumerable<IRole>>(_ctx.Roles, null);
            }
            catch(Exception ex)
            {
                log.Error("Authentication Repository Error", ex);
                return new TransactionResponse<IEnumerable<IRole>>(null, ex);
            }
        }

        public ITransactionResponse<IEnumerable<IRole>> GetRolesByUser(string email)
        {
            try
            {
                User user = _ctx.Users.Include(u => u.Roles).FirstOrDefault(u => u.EmailAddress == email && u.IsEmailVerified);
                if(user == null)
                {
                    return new TransactionResponse<IEnumerable<IRole>>(TransactionStatus.UserNotFound, null, null);
                }
                else
                {
                    return new TransactionResponse<IEnumerable<IRole>>(_ctx.Roles.Where(r => user.Roles.Contains(r)), null);
                }
            }
            catch (Exception ex)
            {
                return new TransactionResponse<IEnumerable<IRole>>(null, ex);
            }
        }

        public ITransactionResponse<IUser> GetUser(Guid id)
        {
            try
            {
                var user = _ctx.Users.Include(u => u.Roles).FirstOrDefault(u => u.Id == id);
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
                var user = _ctx.Users.Include(u => u.Roles).FirstOrDefault(u => u.EmailAddress == email && u.IsEmailVerified);
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

        public ITransactionResponse<IEnumerable<IUser>> GetUsers()
        {
            try
            {
                return new TransactionResponse<IEnumerable<IUser>>(_ctx.Users.Include(u => u.Roles), null);
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
                return new TransactionResponse<IEnumerable<IUser>>(_ctx.Users.Include(u => u.Roles).Where(u => u.IsEmailVerified), null);
            }
            catch (Exception ex)
            {
                log.Error("Authentication Repository Error", ex);
                return new TransactionResponse<IEnumerable<IUser>>(null, ex);
            }
        }

        public ITransactionResponse<IRole> UpdateRole(IRole role)
        {
            using (var transaction = _ctx.Database.BeginTransaction())
            {
                try
                {
                    _ctx.Roles.Update(role as Role);
                    _ctx.SaveChanges();
                    transaction.Commit();
                    return new TransactionResponse<IRole>(_ctx.Roles.Find(role.Id), null);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    log.Error("Authentication Repository Error", ex);
                    return new TransactionResponse<IRole>(null, ex);
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
