using System;
using System.Collections.Generic;
using System.Text;

namespace EasyMicroservices.Cores.Contracts.Requests
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UpdateBulkRequestContract<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public List<T> Items { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>

        public static implicit operator UpdateBulkRequestContract<T>(List<T> items)
        {
            return new UpdateBulkRequestContract<T>()
            {
                Items = items
            };
        }
    }
}
