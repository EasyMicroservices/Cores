using EasyMicroservices.Cores.Database.Interfaces;

namespace EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Entities
{
    public class UserEntity : IIdSchema<long>, IUniqueIdentitySchema
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string UniqueIdentity { get; set; }
    }
}
