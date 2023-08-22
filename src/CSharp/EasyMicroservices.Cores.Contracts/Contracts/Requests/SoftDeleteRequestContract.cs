namespace EasyMicroservices.Cores.Contracts.Requests
{
    /// <summary>
    /// 
    /// </summary>
    public class SoftDeleteRequestContract<T> : DeleteRequestContract<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsDelete { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public static implicit operator SoftDeleteRequestContract<T>((T Id, bool IsDelete) values)
        {
            return new SoftDeleteRequestContract<T>()
            {
                Id = values.Id,
                IsDelete = values.IsDelete
            };
        }
    }
}
