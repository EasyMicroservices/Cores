using EasyMicroservices.Cores.Interfaces;

namespace EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Entities
{
    public class UserEntity : IIdSchema<long>, IUniqueIdentitySchema, IDateTimeSchema, ISoftDeleteSchema
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string UniqueIdentity { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDateTime { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime? ModificationDateTime { get; set; }
        public ICollection<AddressEntity> Addresses { get; set; }
        public ICollection<UserCompanyEntity> UserCompanies { get; set; }
        public ICollection<ProfileEntity> Profiles { get; set; }
    }
}
