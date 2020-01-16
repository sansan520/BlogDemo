using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BlogDemo.Api
{
    public class StartupProduction
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpsRedirection(config => {
                config.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                config.HttpsPort = 5001;  //与launchSettings.json 配置文件保持一致
            });
            // 微软建议生产环境使用hsts
            services.AddHsts(options=> {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
                options.ExcludedHosts.Add("example.com");
                options.ExcludedHosts.Add("www.example.com");
            });
            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            // 微软建议生产环境使用hsts
            app.UseHsts();
            app.UseHttpsRedirection();// https
            app.UseMvc();
        }
    }
}
