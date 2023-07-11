using EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Contexts
{
    public class MyTestContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("MyTestContext");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>(e =>
            {
                e.HasKey(x => x.Id);
            });
        }
    }
}
