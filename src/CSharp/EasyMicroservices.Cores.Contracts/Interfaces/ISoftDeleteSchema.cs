using System;

namespace EasyMicroservices.Cores.Interfaces
{
    /// <summary>
    /// soft delete a row in database
    /// </summary>
    public interface ISoftDeleteSchema
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
