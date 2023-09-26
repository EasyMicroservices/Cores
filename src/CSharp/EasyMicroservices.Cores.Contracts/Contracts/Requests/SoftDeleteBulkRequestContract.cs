using System;
using System.Collections.Generic;
using System.Text;

namespace EasyMicroservices.Cores.Contracts.Requests
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SoftDeleteBulkRequestContract<T> : DeleteBulkRequestContract<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsDelete { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        public static implicit operator SoftDeleteBulkRequestContract<T>(List<T> ids)
        {
            return new SoftDeleteBulkRequestContract<T>()
            {
                Ids = ids,
                IsDelete = true
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public static implicit operator SoftDeleteBulkRequestContract<T>((List<T> Ids, bool IsDelete) values)
        {
            return new SoftDeleteBulkRequestContract<T>()
            {
                Ids = values.Ids,
                IsDelete = values.IsDelete
            };
        }
    }
}
