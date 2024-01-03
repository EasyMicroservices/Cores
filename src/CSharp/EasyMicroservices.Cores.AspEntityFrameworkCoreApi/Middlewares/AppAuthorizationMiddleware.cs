using EasyMicroservices.Cores.AspEntityFrameworkCoreApi.Interfaces;
using EasyMicroservices.ServiceContracts;
using EasyMicroservices.ServiceContracts.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Net.Mime;
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
            try
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

                if (httpContext.Response.StatusCode == 401 || httpContext.Response.StatusCode == 403)
                {
                    httpContext.Response.ContentType = MediaTypeNames.Application.Json;
                    httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                    MessageContract response = FailedReasonType.SessionAccessDenied;
                    response.Error.ServiceDetails.MethodName = httpContext.Request.Path.ToString();
                    response.Error.Details = $"StatusCode: {httpContext.Response.StatusCode}";
                    var json = JsonSerializer.Serialize(response);
                    await httpContext.Response.WriteAsync(json);
                }
                else if (httpContext.Response.StatusCode == 204)
                {
                    httpContext.Response.ContentType = MediaTypeNames.Application.Json;
                    httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                    MessageContract response = FailedReasonType.Nothing;
                    response.Error.Message = "Do not send null value to the service response! always send me a MessageContract";
                    response.Error.ServiceDetails.MethodName = httpContext.Request.Path.ToString();
                    var json = JsonSerializer.Serialize(response);
                    await httpContext.Response.WriteAsync(json);
                }
            }
            catch (Exception ex)
            {
                await ExceptionHandler(httpContext, ex);
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal static Task ExceptionHandler(HttpContext context)
        {
            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            return ExceptionHandler(context, exception);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        internal static Task ExceptionHandler(HttpContext context, Exception exception)
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            MessageContract response;
            if (exception is InvalidResultOfMessageContractException ex)
            {
                response = ex.MessageContract;
                response.Error.StackTrace.Add(ex.Message);
                response.Error.StackTrace.AddRange(ex.StackTrace.ToListStackTrace());
            }
            else
                response = exception;
            if (exception.Message.Contains("Authenti", StringComparison.OrdinalIgnoreCase) && response.Error.FailedReasonType != FailedReasonType.AccessDenied)
                response.Error.FailedReasonType = FailedReasonType.SessionAccessDenied;
            response.Error.ServiceDetails.MethodName = context.Request.Path.ToString();
            var json = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(json);
        }
    }
}
