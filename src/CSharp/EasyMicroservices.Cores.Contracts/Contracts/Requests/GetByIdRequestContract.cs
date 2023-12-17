using EasyMicroservices.Cores.Interfaces;

namespace EasyMicroservices.Cores.Contracts.Requests
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GetByIdRequestContract<T> : IIdSchema<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public T Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public static implicit operator GetByIdRequestContract<T>(T id)
        {
            return new GetByIdRequestContract<T>()
            {
                Id = id
            };
        }
    }
}
