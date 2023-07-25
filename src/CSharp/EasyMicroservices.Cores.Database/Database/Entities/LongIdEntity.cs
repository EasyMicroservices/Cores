using EasyMicroservices.Cores.Interfaces;

namespace EasyMicroservices.Cores.Database.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class LongIdEntity : IIdSchema<long>
    {
        /// <summary>
        /// 
        /// </summary>
        public long Id { get; set; }
    }
}
