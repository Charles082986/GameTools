using System;
using System.Collections.Generic;
using System.Text;

namespace GameTools.Authentication.Interfaces.Repositories
{
    public interface IAuthenticationRepositoryFactory
    {
        IAuthenticationRepository CreateRepository(string connectionString);
    }
}
