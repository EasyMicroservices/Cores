using EasyMicroservices.Cores.AspCoreApi.Interfaces;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.ServiceContracts;
using EasyMicroservices.Utilities.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
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
            BaseUnitOfWork = baseUnitOfWork;
        }

        /// <summary>
        /// 
        /// </summary>
        public IBaseUnitOfWork BaseUnitOfWork { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<MessageContract> CheckIsAuthorized(HttpContext httpContext)
        {
            await httpContext.AuthenticateAsync("Bearer");
            var hasPermission = await HasPermission(httpContext);
            if (!hasPermission.IsSuccess)
                return hasPermission.ToContract();
            else if (!hasPermission.Result)
                return (FailedReasonType.AccessDenied, "Sorry, you cannot call this service, you have not enough permissions!");
            return true;
        }

        async Task<string> GetMicroserviceName()
        {
            return (await BaseUnitOfWork.InitializeWhiteLabel()).MicroserviceName;
        }

        static ConcurrentDictionary<string, ICollection<ServicePermissionContract>> CachedPermissions { get; set; } = new();
        static readonly ConcurrentTreeDictionary TreeDictionary = new ConcurrentTreeDictionary();
        async Task<MessageContract> FetchData(string roleName)
        {
            var authenticationRouteAddress = BaseUnitOfWork.GetServiceAddress("Authentication")?.Address;
            var httpClient = new System.Net.Http.HttpClient();
            var servicePermissionClient = new Authentications.GeneratedServices.ServicePermissionClient(authenticationRouteAddress, httpClient);
            //StringContent content = new StringContent(JsonSerializer.Serialize(new Authentications.GeneratedServices.ServicePermissionRequestContract()
            //{
            //    MicroserviceName = await GetMicroserviceName(),
            //    RoleName = roleName
            //}), Encoding.UTF8, "application/json");
            //var ress = await httpClient.PostAsync($"{authenticationRouteAddress}/api/servicepermission/GetAllPermissionsBy", content);
            //var dd = await ress.Content.ReadAsStringAsync();
            var permissionsResult = await servicePermissionClient.GetAllPermissionsByAsync(new Authentications.GeneratedServices.ServicePermissionRequestContract()
            {
                MicroserviceName = await GetMicroserviceName(),
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

        MessageContract<bool> IsAnonymousMethodCalling(HttpContext httpContext)
        {
            var endpoints = httpContext.GetEndpoint();
            if (endpoints == null)
                return true;
            var controllerActionDescriptor = httpContext.GetEndpoint().Metadata.GetMetadata<ControllerActionDescriptor>();
            if (controllerActionDescriptor == null)
                return (FailedReasonType.Nothing, "controllerActionDescriptor is null or empty, did you sent correct route to me?");
            if (controllerActionDescriptor.ControllerTypeInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute)).Any() ||
                controllerActionDescriptor.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute)).Any())
                return true;
            return false;
        }

        async Task<MessageContract<bool>> HasPermission(HttpContext httpContext)
        {
            var isAnonymousMethodCalling = IsAnonymousMethodCalling(httpContext);
            if (!isAnonymousMethodCalling.IsSuccess)
                return isAnonymousMethodCalling;
            else if (isAnonymousMethodCalling.Result)
                return true;
            string controllerName = httpContext.Request.RouteValues["controller"].ToString();
            string actionName = httpContext.Request.RouteValues["action"].ToString();
            List<Claim> roleClaims = httpContext.User.FindAll(ClaimTypes.Role).ToList();
            if (roleClaims.Count == 0)
                return (FailedReasonType.AccessDenied, $"There is no claim role founded! did you forgot to use services.AddAuthentication? or did you set Bearer for authorize? controllerName: {controllerName} actionName: {actionName}");
            if (!roleClaims.All(x => CachedPermissions.ContainsKey(x.Value)))
            {
                foreach (var role in roleClaims)
                {
                    var fetchDataResult = await FetchData(role.Value);
                    if (!fetchDataResult)
                        return fetchDataResult.ToContract<bool>();
                }
            }
            string microserviceName = await GetMicroserviceName();
            var qq = TreeDictionary.TryGetValue(["Owner", microserviceName, controllerName, actionName], out IList<object> permissions);

            return roleClaims.Any(role => TreeDictionary.TryGetValue([role.Value, microserviceName, controllerName, actionName], out IList<object> permissions)
                 && permissions.LastOrDefault() is bool value && value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task<bool> HasUnlimitedPermission(HttpContext httpContext)
        {
            if (httpContext == null)
                return false;
            else if (IsAnonymousMethodCalling(httpContext).Result)
                return true;
            List<Claim> roleClaims = httpContext.User.FindAll(ClaimTypes.Role).ToList();
            if (roleClaims.Count == 0)
                return false;
            if (!roleClaims.All(x => CachedPermissions.ContainsKey(x.Value)))
            {
                foreach (var role in roleClaims)
                {
                    var fetchDataResult = await FetchData(role.Value);
                    if (!fetchDataResult)
                        return false;
                }
            }
            return roleClaims.Any(role => TreeDictionary.TryGetValue([role.Value, null, null, null], out IList<object> permissions)
                  && permissions.LastOrDefault() is bool value && value);
        }
    }
}
