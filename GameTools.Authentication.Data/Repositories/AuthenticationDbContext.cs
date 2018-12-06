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
            modelBuilder.Entity<Role>().HasKey("Id");
            modelBuilder.Entity<Role>().HasIndex("Name").IsUnique();

            modelBuilder.Entity<User>().HasKey("Id");
            modelBuilder.Entity<User>().HasIndex("EmailAddress").IsUnique();
            modelBuilder.Entity<User>().HasMany("Role");
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
