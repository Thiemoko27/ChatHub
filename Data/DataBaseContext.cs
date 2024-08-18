using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ChatHub.Models;

namespace ChatHub.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<UserDataBaseContext>
    {
        public UserDataBaseContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("UserConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'UserConnection' Not found.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<UserDataBaseContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new UserDataBaseContext(optionsBuilder.Options);
        }
    }

    public class UserDataBaseContext : DbContext {
        public UserDataBaseContext(DbContextOptions<UserDataBaseContext> options) : base(options) { }
        public DbSet<User> Users { get; set; } = null!;
    }

    public class MessageDataBaseContext : DbContext {
        public MessageDataBaseContext(DbContextOptions<MessageDataBaseContext> options) : base(options) { }
        public DbSet<Message> Messages { get; set; } = null!;
    }
}