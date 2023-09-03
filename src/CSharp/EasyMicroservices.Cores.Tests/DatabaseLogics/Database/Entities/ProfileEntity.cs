using EasyMicroservices.Cores.Database.Schemas;

namespace EasyMicroservices.Cores.Tests.DatabaseLogics.Database.Entities
{
    public class ProfileEntity : FullAbilityIdSchema<long>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public long UserId { get; set; }
        public UserEntity User { get; set; }
    }
}
