using System;
using System.Data.Common;
using AuthServer.Server.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AuthServer.Server.Tests
{
    public class SharedDatabaseFixture : IDisposable
    {
        private static readonly object _lock = new object();
        private static bool _databaseInitialized;

        public SharedDatabaseFixture()
        {
            Connection = new NpgsqlConnection(@"Host=localhost;Database=testdb;Username=postgres;Password=example");

            Seed();

            Connection.Open();
        }

        public DbConnection Connection { get; }

        public AuthDbContext CreateContext(DbTransaction transaction = null)
        {
            var context = new AuthDbContext(new DbContextOptionsBuilder<AuthDbContext>().UseNpgsql(Connection, o => o.UseNodaTime()).Options);

            if (transaction != null)
            {
                context.Database.UseTransaction(transaction);
            }

            return context;
        }

        private void Seed()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();

                        AppUser user1 = new AppUser { UserName = "Test User 1", Email = "test1@example.com" };
                        AppUser user2 = new AppUser { UserName = "Test User 2", Email = "test2@example.com" };
                        context.AddRange(user1, user2);

                        context.SaveChanges();
                    }

                    _databaseInitialized = true;
                }
            }
        }

        public void Dispose()
        {
            Connection.Dispose();
        }
    }
}
