using EasyMicroservices.Cores.Interfaces;
using System;

namespace EasyMicroservices.Cores.Database.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeEntity : IDateTimeSchema
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
