using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AuthServer.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AuthServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddIdentityServer()
                //在每次启动时，为令牌签名创建了一个临时密钥。在生成环境需要一个持久化的密钥
                //Sets the temporary signing credential. 设置临时签名证书
                .AddDeveloperSigningCredential()
                // api资源
                .AddInMemoryApiResources(InMemoryConfiguration.ApiResources())
                .AddInMemoryClients(InMemoryConfiguration.Clients())
                //添加测试用户;
                .AddTestUsers(InMemoryConfiguration.Users().ToList())
                //身份资源
                .AddInMemoryIdentityResources(InMemoryConfiguration.GetIdentityResources());

            //services.AddIdentityServer()
            //     //.AddDeveloperSigningCredential()
            //     .AddSigningCredential(new X509Certificate2(@"F:\王槐三\C#10\GitHub_ Repository\BlogDemo\AuthServer\win_openssl\socialnetwork.pfx", "password"))
            //    .AddInMemoryIdentityResources(InMemoryConfiguration.GetIdentityResources())
            //    .AddTestUsers(InMemoryConfiguration.Users().ToList())
            //    .AddInMemoryClients(InMemoryConfiguration.Clients())
            //    .AddInMemoryApiResources(InMemoryConfiguration.ApiResources());

            //services.AddAuthentication("Bearer");

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("js_oidc", policy =>
            //    {
            //        policy.WithOrigins("http://localhost:5003")
            //            .AllowAnyHeader()
            //            .AllowAnyMethod();
            //    });
            //});
            //#region xxx
            //var builder = services.AddIdentityServer(options =>
            //{
            //    options.Events.RaiseErrorEvents = true;
            //    options.Events.RaiseInformationEvents = true;
            //    options.Events.RaiseFailureEvents = true;
            //    options.Events.RaiseSuccessEvents = true;
            //})
            //    .AddInMemoryIdentityResources(InMemoryConfiguration.GetIdentityResources())
            //    .AddInMemoryApiResources(InMemoryConfiguration.ApiResources())
            //    .AddInMemoryClients(InMemoryConfiguration.Clients())
            //    .AddTestUsers(InMemoryConfiguration.Users().ToList());


            // builder.AddDeveloperSigningCredential();


            ////services.AddAuthentication();
            ////.AddGoogle(options =>
            ////{
            ////    options.ClientId = "708996912208-9m4dkjb5hscn7cjrn5u0r4tbgkbj1fko.apps.googleusercontent.com";
            ////    options.ClientSecret = "wdfPY6t8H8cecgjlxud__4Gh";
            ////});
            //services.AddAuthentication("Bearer");

            //services.AddHsts(options =>
            //{
            //    options.Preload = true;
            //    options.IncludeSubDomains = true;
            //    options.MaxAge = TimeSpan.FromDays(60);
            //    // options.ExcludedHosts.Add("example.com");
            //    // options.ExcludedHosts.Add("www.example.com");
            //});

            //services.AddHttpsRedirection(options =>
            //{
            //    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            //    options.HttpsPort = 5001;
            //});


            //services.AddCors(options =>
            //{
            //    options.AddPolicy("AngularDev", policy =>
            //    {
            //        policy.WithOrigins("http://localhost:5003")
            //            .AllowAnyHeader()
            //            .AllowAnyMethod();
            //    });
            //}); 
            //#endregion

            //services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseCors();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
          
            app.UseIdentityServer();
            app.UseMvcWithDefaultRoute();
        }
    }
}
