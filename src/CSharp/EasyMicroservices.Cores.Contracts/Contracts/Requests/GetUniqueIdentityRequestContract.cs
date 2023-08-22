using EasyMicroservices.Cores.Interfaces;

namespace EasyMicroservices.Cores.Contracts.Requests
{
    /// <summary>
    /// 
    /// </summary>
    public class GetUniqueIdentityRequestContract : IUniqueIdentitySchema
    {
        /// <summary>
        /// 
        /// </summary>
        public string UniqueIdentity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueIdentity"></param>
        public static implicit operator GetUniqueIdentityRequestContract(string uniqueIdentity)
        {
            return new GetUniqueIdentityRequestContract()
            {
                UniqueIdentity = uniqueIdentity
            };
        }
    }
}
