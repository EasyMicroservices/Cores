using EasyMicroservices.Cores.Interfaces;
using System;

namespace EasyMicroservices.Cores.Database.Schemas
{
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeSchema : IDateTimeSchema
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
