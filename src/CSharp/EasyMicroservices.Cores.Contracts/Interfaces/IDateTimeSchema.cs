using System;

namespace EasyMicroservices.Cores.Interfaces
{
    /// <summary>
    /// Date and time of creation and modification
    /// </summary>
    public interface IDateTimeSchema
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreationDateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? ModificationDateTime { get; set; }
    }
}
