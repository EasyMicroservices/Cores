using System;
using System.Collections.Generic;
using System.Text;

namespace EasyMicroservices.Cores.Contracts.Requests
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CreateBulkRequestContract<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public List<T> Items { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>

        public static implicit operator CreateBulkRequestContract<T>(List<T> items)
        {
            return new CreateBulkRequestContract<T>()
            {
                Items = items
            };
        }
    }
}
