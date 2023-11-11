using EasyMicroservices.Cores.AspCoreApi.Interfaces;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.ServiceContracts;
using EasyMicroservices.Utilities.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using ServicePermissionContract = Authentications.GeneratedServices.ServicePermissionContract;
namespace EasyMicroservices.Cores.AspCoreApi.Authorizations
{
    /// <summary>
    /// 
    /// </summary>
    public class AspCoreAuthorization : IAuthorization
    {
        /// <summary>
        /// 
        /// </summary>
        public AspCoreAuthorization(IBaseUnitOfWork baseUnitOfWork)
        {
            UniqueIdentityManager = baseUnitOfWork.GetUniqueIdentityManager();
        }

        /// <summary>
        /// 
        /// </summary>
        public IUniqueIdentityManager UniqueIdentityManager { get; }
        /// <summary>
        /// 
        /// </summary>
        public static string AuthenticationRouteAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<MessageContract> CheckIsAuthorized(HttpContext httpContext)
        {
            var hasPermission = await HasPermission(httpContext);
            if (!hasPermission.IsSuccess)
                return hasPermission.ToContract();
            else if (!hasPermission.Result)
                return (FailedReasonType.AccessDenied, "Sorry, you cannot call this service, you have not enough permissions!");
            return true;
        }

        static ConcurrentDictionary<string, ICollection<ServicePermissionContract>> CachedPermissions { get; set; } = new();
        static readonly ConcurrentTreeDictionary TreeDictionary = new ConcurrentTreeDictionary();
        async Task<MessageContract> FetchData(string roleName)
        {
            var servicePermissionClient = new Authentications.GeneratedServices.ServicePermissionClient(AuthenticationRouteAddress, new System.Net.Http.HttpClient());
            var permissionsResult = await servicePermissionClient.GetAllPermissionsByAsync(new Authentications.GeneratedServices.ServicePermissionRequestContract()
            {
                MicroserviceName = UniqueIdentityManager.MicroserviceName,
                RoleName = roleName
            });
            if (permissionsResult.IsSuccess)
            {
                CachedPermissions[roleName] = permissionsResult.Result;
                AddToTreeDictionary(roleName, permissionsResult.Result);
                return true;
            }
            return permissionsResult.ToContract();
        }

        void AddToTreeDictionary(string roleName, ICollection<ServicePermissionContract> permissions)
        {
            foreach (var permission in permissions)
            {
                TreeDictionary.TryAdd(roleName, permission.MicroserviceName, permission.ServiceName, permission.MethodName, true);
            }
        }

        async Task<MessageContract<bool>> HasPermission(HttpContext httpContext)
        {
            var endpoints = httpContext.GetEndpoint();
            if (endpoints == null)
                return true;
            var controllerActionDescriptor = httpContext.GetEndpoint().Metadata.GetMetadata<ControllerActionDescriptor>();
            if (controllerActionDescriptor.ControllerTypeInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute)).Any() ||
                controllerActionDescriptor.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute)).Any())
                return true;

            string controllerName = httpContext.Request.RouteValues["controller"].ToString();
            string actionName = httpContext.Request.RouteValues["action"].ToString();
            List<Claim> roleClaims = httpContext.User.FindAll(ClaimTypes.Role).ToList();
            if (roleClaims.Count == 0)
                return (FailedReasonType.AccessDenied, "There is no claim role founded! did you forgot to use services.AddAuthentication?");
            if (!roleClaims.All(x => CachedPermissions.ContainsKey(x.Value)))
            {
                foreach (var role in roleClaims)
                {
                    var fetchDataResult = await FetchData(role.Value);
                    if (!fetchDataResult)
                        return fetchDataResult.ToContract<bool>();
                }
            }
            return roleClaims.Any(role => TreeDictionary.TryGetValue(new object[] { role.Value, UniqueIdentityManager.MicroserviceName, controllerName, actionName }, out IList<object> permissions)
                 && permissions.LastOrDefault() is bool value && value);
        }
    }
}
