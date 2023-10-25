using EasyMicroservices.ServiceContracts;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.AspCoreApi.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAuthorization
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        Task<MessageContract> CheckIsAuthorized(HttpContext httpContext);
    }
}
