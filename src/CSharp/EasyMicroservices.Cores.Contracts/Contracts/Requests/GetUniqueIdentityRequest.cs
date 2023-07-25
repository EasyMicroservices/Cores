using EasyMicroservices.Cores.Interfaces;

namespace EasyMicroservices.Cores.Contracts.Contracts.Requests
{
    /// <summary>
    /// 
    /// </summary>
    public class GetUniqueIdentityRequest : IUniqueIdentitySchema
    {
        /// <summary>
        /// 
        /// </summary>
        public string UniqueIdentity { get; set; }
    }
}
