using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlackFolderGames.Data.Context
{
    public class BlackFolderGamesDbContext : DbContext
    {
        public DbSet<Campaign> Campaings { get; set; }
    }
}
