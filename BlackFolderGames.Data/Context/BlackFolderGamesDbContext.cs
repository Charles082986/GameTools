using BlackFolderGames.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlackFolderGames.Data.Context
{
    public class BlackFolderGamesDbContext : DbContext
    {
        public BlackFolderGamesDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Campaign> Campaings { get; set; }
        public DbSet<CampaignSetting> CampaignSettings { get; set; }
        public DbSet<ImageLog> ImageLogs { get; set; }
    }
}
