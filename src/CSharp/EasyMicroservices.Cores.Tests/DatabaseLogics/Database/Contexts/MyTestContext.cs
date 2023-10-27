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
        public DbSet<ProfileEntity> Profiles { get; set; }
        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<SubjectEntity> Subjects { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<RoleParentChildEntity> RoleParentChildren { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_builder != null)
                _builder.OnConfiguring(optionsBuilder);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserCompanyEntity>(e =>
            {
                e.HasKey(x => new { x.UserId, x.CompanyId });
            });

            modelBuilder.Entity<RoleParentChildEntity>(e =>
            {
                e.HasKey(x => new { x.ParentId, x.ChildId });

                e.HasOne(x => x.Child)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ChildId);

                e.HasOne(x => x.Parent)
                .WithMany(x => x.Parents)
                .HasForeignKey(x => x.ParentId);
            });
            var builder = base.AutoModelCreating(modelBuilder);

            Assert.Equal(@"User-Addresses-UserId
User-Profiles-UserId
User-UserCompanies-UserId
Company-UserCompanies-CompanyId
", builder.ToString());
        }
    }
}
