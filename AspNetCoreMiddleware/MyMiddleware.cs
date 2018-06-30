using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNetCoreMiddleware
{
    public class MyMiddleware
    {
        private RequestDelegate _next;
        private ILoggerFactory _loggerFactory;
        private IOptions<MyMiddlewareOptions> _options;

        public MyMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IOptions<MyMiddlewareOptions> options)
        {
            _next = next;
            _loggerFactory = loggerFactory;
            _options = options;
        }

        public async Task Invoke(HttpContext context)
        {
            _loggerFactory.AddConsole();
            var logger = _loggerFactory.CreateLogger("My Own Logger");
            logger.LogInformation("My middleware is handling the request");
            await context.Response.WriteAsync(_options.Value.OptionOne + Environment.NewLine);
            await context.Response.WriteAsync("My middleware is handling the request!" + Environment.NewLine);
            await _next.Invoke(context);
            await context.Response.WriteAsync("My middleware has completed handing the request!" + Environment.NewLine);

            //pass data to other middleware
            context.Items["message"] = "The weather today is hot!";
        }
    }

    public class MyMiddlewareOptions
    {
        public string OptionOne { get; set; }
    }
    
    public static class MyMiddlewareExtensions
    {
        public static IApplicationBuilder UseMyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MyMiddleware>();
        }
    }

}