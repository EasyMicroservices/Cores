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
    public class AppGlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public AppGlobalExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            await _next(context);
            //if (context.Response.StatusCode == 500)
            //{
            //    context.Response.ContentType = MediaTypeNames.Application.Json;
            //    context.Response.StatusCode = (int)HttpStatusCode.OK;
            //    var bytes = await EasyMicroservices.Utilities.IO.StreamExtensions.StreamToBytesAsync(context.Response.HttpContext.Response.Body, context.Response.ContentLength.Value, 1024);
            //    //MessageContract response = exception is InvalidResultOfMessageContractException ex ? ex.MessageContract : exception;
            //    //if (exception.Message.Contains("Authenti", StringComparison.OrdinalIgnoreCase))
            //    //    response.Error.FailedReasonType = FailedReasonType.SessionAccessDenied;
            //    //response.Error.ServiceDetails.MethodName = context.Request.Path.ToString();
            //    //var json = JsonSerializer.Serialize(response);
            //    //await context.Response.WriteAsync(json);
            //}
        }
    }
}
