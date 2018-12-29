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
            Security security = new Security(1000, 256);
            return new AuthenticationRepository(connectionString, security);
        }
    }
}
