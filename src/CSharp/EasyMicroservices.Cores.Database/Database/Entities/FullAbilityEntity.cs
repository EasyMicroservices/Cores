using EasyMicroservices.Cores.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyMicroservices.Cores.Database.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class FullAbilityEntity : IUniqueIdentitySchema, ISoftDeleteSchema, IDateTimeSchema
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreationDateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? ModificationDateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? DeletedDateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UniqueIdentity { get; set; }
    }
}
