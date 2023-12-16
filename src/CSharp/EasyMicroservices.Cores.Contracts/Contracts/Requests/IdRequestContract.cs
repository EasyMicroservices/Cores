using EasyMicroservices.Cores.Interfaces;

namespace EasyMicroservices.Cores.Contracts.Requests
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IdRequestContract<T> : IIdSchema<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public T Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public static implicit operator IdRequestContract<T>(T id)
        {
            return new IdRequestContract<T>()
            {
                Id = id
            };
        }
    }
}
