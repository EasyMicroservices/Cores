using EasyMicroservices.Cores.Interfaces;

namespace EasyMicroservices.Cores.Database.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public class FullAbilityIdEntity<TId> : FullAbilityEntity, IIdSchema<TId>
    {
        /// <summary>
        /// 
        /// </summary>
        public TId Id { get; set; }
    }
}
