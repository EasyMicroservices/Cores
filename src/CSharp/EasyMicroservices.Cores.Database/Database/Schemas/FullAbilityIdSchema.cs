using EasyMicroservices.Cores.Interfaces;

namespace EasyMicroservices.Cores.Database.Schemas
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public class FullAbilityIdSchema<TId> : FullAbilitySchema, IIdSchema<TId>
    {
        /// <summary>
        /// 
        /// </summary>
        public TId Id { get; set; }
    }
}
