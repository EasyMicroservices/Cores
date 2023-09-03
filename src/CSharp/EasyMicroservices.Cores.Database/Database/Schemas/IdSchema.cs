using EasyMicroservices.Cores.Interfaces;

namespace EasyMicroservices.Cores.Database.Schemas
{
    /// <summary>
    /// 
    /// </summary>
    public class IdSchema<TId> : IIdSchema<TId>
    {
        /// <summary>
        /// 
        /// </summary>
        public TId Id { get; set; }
    }
}
