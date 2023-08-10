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
    }
}
