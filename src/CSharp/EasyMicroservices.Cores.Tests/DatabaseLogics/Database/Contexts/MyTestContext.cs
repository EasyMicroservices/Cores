using EasyMicroservices.Cores.Relational.EntityFrameworkCore;
using EasyMicroservices.Cores.Relational.EntityFrameworkCore.Intrerfaces;
using EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Contexts
{
    public class MyTestContext : RelationalCoreContext
    {
        IEntityFrameworkCoreDatabaseBuilder _builder;
        public MyTestContext(IEntityFrameworkCoreDatabaseBuilder builder)
        {
            _builder = builder;
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<AddressEntity> Addresses { get; set; }
        public DbSet<CompanyEntity> Companies { get; set; }
        public DbSet<UserCompanyEntity> UserCompanies { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_builder != null)
                _builder.OnConfiguring(optionsBuilder);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var builder = base.AutoModelCreating(modelBuilder);
            modelBuilder.Entity<UserCompanyEntity>(e =>
            {
                e.HasKey(x => new { x.UserId, x.CompanyId });
            });

            Assert.Equal(@"User-Addresses-UserId
User-UserCompanies-UserId
Company-UserCompanies-CompanyId
", builder.ToString());
        }
    }
}
