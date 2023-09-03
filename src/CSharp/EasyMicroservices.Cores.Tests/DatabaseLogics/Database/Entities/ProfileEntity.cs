using EasyMicroservices.Cores.Database.Entities;

namespace EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Entities
{
    public class ProfileEntity : FullAbilityIdEntity<long>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public long UserId { get; set; }
        public UserEntity User { get; set; }
    }
}
