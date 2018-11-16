using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Infrastructure.DataBase;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Blog.Core.Repositories;
using Blog.Infrastructure.Repositories;
using Blog.Core;
using Blog.Infrastructure;

namespace BlogDemo.Api
{
    public class StartupDevelopment
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // 添加服务的先后顺序无关系
            services.AddHttpsRedirection(config => {
                config.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                config.HttpsPort = 5001;  //与launchSettings.json 配置文件保持一致
            });
            services.AddMvc();

            services.AddDbContext<MyContext>(options=> {
                options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=BlogDemo;Trusted_Connection=True;MultipleActiveResultSets=true");
            });

            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            // 管道先后顺序需要注意
            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();// https
            app.UseMvc();
        }
    }
}
