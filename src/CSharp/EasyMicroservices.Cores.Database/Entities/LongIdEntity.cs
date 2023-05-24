using EasyMicroservices.Cores.Database.Interfaces;

namespace EasyMicroservices.Cores.Database.Entities
{
    public class LongIdEntity : IIdSchema<long>
    {
        public long Id { get; set; }
    }
}
