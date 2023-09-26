using EasyMicroservices.Cores.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyMicroservices.Cores.Contracts.Requests
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteBulkRequestContract<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public List<T> Ids { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        public static implicit operator DeleteBulkRequestContract<T>(List<T> ids)
        {
            return new DeleteBulkRequestContract<T>()
            {
                Ids = ids
            };
        }
    }
}
