using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BlogDemo.Api.Extensions
{
   /// <summary>
   /// 自定义扩展类
   /// </summary>
    public static class ExceptionHandlingExtensions
    {
        public static void UseHuaisanExceptionHandler(this IApplicationBuilder app,ILoggerFactory loggerFactory)
        {
            app.UseExceptionHandler(options=> {
                options.Run(async context=> {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    var ex = context.Features.Get<IExceptionHandlerFeature>();
                    if (ex != null)
                    {
                        var logger = loggerFactory.CreateLogger("BlogDemo.Api.Extensions.ExceptionHandlingExtensions");
                        logger.LogError(500, ex.Error, ex.Error.Message);
                    }

                    await context.Response.WriteAsync(ex?.Error?.Message ?? "An Error Occurred.");
                });
            });
        }
    }
}
