using EasyMicroservices.Cores.Interfaces;
using System;

namespace EasyMicroservices.Cores.Database.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class SoftDeleteEntity : ISoftDeleteSchema
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? DeletedDateTime { get; set; }
    }
}
