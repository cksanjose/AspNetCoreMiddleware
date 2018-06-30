using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreMiddleware
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            var myMiddlewareOptions = Configuration.GetSection("MyMiddlewareOptionsSection");
            services.Configure<MyMiddlewareOptions>(o => o.OptionOne = myMiddlewareOptions["OptionOne"]);
        }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appSettings.json", false, true);

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("Hello from component one!" + Environment.NewLine);
                await next.Invoke();
                await context.Response.WriteAsync("Hello from component one again!" + Environment.NewLine + context.Items["message"]);
            });

            app.UseMyMiddleware();
            
            //Use Map
            app.Map("/mymapbranch", appBuilder =>
            {
               appBuilder.Run(async (context) =>
                   {
                       await context.Response.WriteAsync("Greetings from my branch" + Environment.NewLine);
                   }); 
            });
            
            //Use MapWhen
            app.MapWhen(context => context.Request.Query.ContainsKey("querybranch"), (appBuilder) =>
            {
               appBuilder.Run(async context =>
                   {
                       await context.Response.WriteAsync("You have arrived at your Map When Branch" + Environment.NewLine);
                   }); 
            });

            app.Run(async (context) => { await context.Response.WriteAsync("Hello World!"+ Environment.NewLine); });
        }
    }
}