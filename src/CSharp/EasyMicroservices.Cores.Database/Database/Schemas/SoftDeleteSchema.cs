using EasyMicroservices.Cores.Interfaces;
using System;

namespace EasyMicroservices.Cores.Database.Schemas
{
    /// <summary>
    /// 
    /// </summary>
    public class SoftDeleteSchema : ISoftDeleteSchema
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
