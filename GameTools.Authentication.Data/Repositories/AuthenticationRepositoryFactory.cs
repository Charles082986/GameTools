using GameTools.Authentication.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTools.Authentication.Data.Repositories
{
    public class AuthenticationRepositoryFactory : IAuthenticationRepositoryFactory
    {
        public IAuthenticationRepository CreateRepository(string connectionString)
        {
            return new AuthenticationRepository(connectionString);
        }
    }
}
