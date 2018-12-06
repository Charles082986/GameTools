using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTools.Authentication.Data.Repositories
{
    internal class AuthenticationDbContextFactory
    {
        internal AuthenticationDbContext CreateDbContext(string connectionString)
        {
            DbContextOptionsBuilder<AuthenticationDbContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<AuthenticationDbContext>();
            dbContextOptionsBuilder.UseSqlServer(connectionString);
            return new AuthenticationDbContext(dbContextOptionsBuilder.Options);
        }
    }
}
