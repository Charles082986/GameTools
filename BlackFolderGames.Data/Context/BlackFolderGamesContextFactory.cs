using BlackFolderGames.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlackFolderGames.Data.Context
{
    public class BlackFolderGamesContextFactory : IBlackFolderGamesContextFactory
    {
        private string _connectionString;
        public BlackFolderGamesContextFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public BlackFolderGamesDbContext Create()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(_connectionString);
            return new BlackFolderGamesDbContext(optionsBuilder.Options);
        }
    }
}
