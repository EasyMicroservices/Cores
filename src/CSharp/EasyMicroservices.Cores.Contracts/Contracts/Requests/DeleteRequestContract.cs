using EasyMicroservices.Cores.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyMicroservices.Cores.Contracts.Requests
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteRequestContract<T> : IIdSchema<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public T Id { get; set; }
    }
}
