using EasyMicroservices.Cores.DataTypes;
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
        /// you will get by unique identity for cvhild and parent system without get childs
        /// when you set it true, you will get only parents
        /// </summary>
        public GetUniqueIdentityType Type { get; set; }
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
