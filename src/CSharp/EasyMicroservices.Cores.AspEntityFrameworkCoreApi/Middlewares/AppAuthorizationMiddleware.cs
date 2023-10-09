using EasyMicroservices.Cores.AspEntityFrameworkCoreApi.Interfaces;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.ServiceContracts.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.AspEntityFrameworkCoreApi.Middlewares
{
    /// <summary>
    /// 
    /// </summary>
    public class AppAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public AppAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="baseUnitOfWork"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext, IUnitOfWork baseUnitOfWork)
        {
            var authorization = baseUnitOfWork.GetAuthorization();
            if (authorization != null)
            {
                var authorizationResult = await authorization.CheckIsAuthorized(httpContext);
                if (!authorizationResult)
                {
                    httpContext.Response.ContentType = MediaTypeNames.Application.Json;
                    httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                    authorizationResult.Error.ServiceDetails.MethodName = httpContext.Request.Path.ToString();
                    var json = JsonSerializer.Serialize(authorizationResult);
                    await httpContext.Response.WriteAsync(json);
                    return;
                }
            }
            await _next(httpContext);
        }
    }
}
