using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace DBLibrary
{
    public class DB:DbContext
    {
        public DbSet<Users> Users { get; set; } = null!;
        public DB() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=AppDB.db");
        }

    }

    public class Users
    {
        public int Id { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }

    }
}
