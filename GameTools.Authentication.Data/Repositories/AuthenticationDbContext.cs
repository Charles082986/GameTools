using GameTools.Authentication.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTools.Authentication.Data.Repositories
{
    internal class AuthenticationDbContext : DbContext
    {
        internal AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : base(options) { }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Claim>().HasKey(c => c.Id);
            modelBuilder.Entity<Claim>().HasIndex("Name", "Provider").IsUnique();
            modelBuilder.Entity<Claim>().HasOne(c => c.User).WithMany(u => u.Claims);

            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().HasIndex(u => u.EmailAddress).IsUnique();

            modelBuilder.Entity<ThirdPartyAuthenticationToken>().HasKey(tpat => tpat.Id);
            modelBuilder.Entity<ThirdPartyAuthenticationToken>()
                .HasOne(tpat => tpat.User)
                .WithMany(u => u.ThirdPartyAuthenticationTokens);
            modelBuilder.Entity<ThirdPartyAuthenticationToken>().HasIndex(tpat => tpat.Token).IsUnique();

            modelBuilder.Entity<ThirdPartyValidationKey>().HasKey("Provider", "Name");
        }

        public DbSet<Claim> Claims { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ThirdPartyAuthenticationToken> ThirdPartyAuthenticationTokens { get; set; }
        public DbSet<ThirdPartyValidationKey> ThirdPartyValidationKeys { get; set; }
    }
}
