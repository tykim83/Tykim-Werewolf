using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Werewolf.Models;

namespace Werewolf.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Game> Game { get; set; }
        public DbSet<GameUser> GameUser { get; set; }
        public DbSet<Note> Note { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Important to add the base OnModelCreating
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GameUser>().HasKey(ba => new { ba.GameId, ba.ApplicationUserId });
        }
    }
}