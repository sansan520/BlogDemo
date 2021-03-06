﻿using Blog.Infrastructure.DataBase;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Blog.Core.Repositories;
using Blog.Infrastructure.Repositories;
using Blog.Core;
using Blog.Infrastructure;
using BlogDemo.Api.Extensions;
using Microsoft.Extensions.Logging;
using AutoMapper;
using FluentValidation;
using Blog.Infrastructure.Resources;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Blog.Infrastructure.HelperServices;
using Newtonsoft.Json.Serialization;
using FluentValidation.AspNetCore;
using System.Linq;

namespace BlogDemo.Api
{
    public class StartupDevelopment
    {
        public static IConfiguration _configuration { get; set; }

        public StartupDevelopment(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // 添加服务的先后顺序无关系
            services.AddHttpsRedirection(config => {
                config.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                config.HttpsPort = 5001;  //与launchSettings.json 配置文件保持一致
            });
            //启用授权
            services.AddAuthorization();
            //启用验证
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options=> {
                    options.RequireHttpsMetadata = true;
                    options.Authority = "https://localhost:5001";
                    options.ApiName = "socialnetwork";
                });
            services.AddMvc(options=> {
                // 启用http 内容协商，客户端 accept header 支持406
                options.ReturnHttpNotAcceptable = true;
                //net core 默认返回格式为json,这里添加xml格式 key=Accept value=application/xml
                //options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                //自定义Media Type
                var intputFormatter = options.InputFormatters.OfType<JsonInputFormatter>().FirstOrDefault();
                if (intputFormatter != null)
                {
                    intputFormatter.SupportedMediaTypes.Add("application/vnd.huaisan.post.create+json");
                    intputFormatter.SupportedMediaTypes.Add("application/vnd.huaisan.post.update+json");
                }

                var outputFormatter = options.OutputFormatters.OfType<JsonOutputFormatter>().FirstOrDefault();
                if (outputFormatter != null)
                {
                    outputFormatter.SupportedMediaTypes.Add("application/vnd.huaisan.hateoas+json");
                }
            })
            .AddJsonOptions(options=> {
                // 确保json返回为驼峰类型，即首字母小写
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            })
            .AddFluentValidation();
            
            #region 读取json配置文件
            //services.AddDbContext<MyContext>(options=> {
            //    options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=BlogDemo;Trusted_Connection=True;MultipleActiveResultSets=true");
            //});
            //方法一：
            //var connectionString = _configuration["ConnectionStrings:DefaultConnection"];
            //方法二：
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<MyContext>(options =>
            {
                options.UseSqlServer(connectionString);
            }); 

            #endregion

            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            //使用aotumapper
            services.AddAutoMapper();
            //fluentValidation
            services.AddTransient<IValidator<PostAddResource>, PostAddOrUpdateResourceValidator<PostAddResource>>();
            services.AddTransient<IValidator<PostUpdateResource>, PostAddOrUpdateResourceValidator<PostUpdateResource>>();

            //添加urlhelper
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });
            //添加自定义属性排序容器
            var propertyMappingContainer = new PropertyMappingContainer();
            propertyMappingContainer.Register<PostPropertyMapping>();
            services.AddSingleton<IPropertyMappingContainer>(propertyMappingContainer);
            //添加自定义方法,controller ctor方法注入时使用
            services.AddTransient<ITypeService, TypeService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,ILoggerFactory loggerFactory)
        {
            // 管道先后顺序需要注意
            //app.UseDeveloperExceptionPage();//为开发人员启用的错误页面，方便查看错误以及调试，正式环境可取消<-> webapi 可用也可不用，webapi 可返回自定义的错误码等
            app.UseHuaisanExceptionHandler(loggerFactory);
            app.UseHttpsRedirection();// https
            //这句话就是在把验证中间件添加到管道里, 这样每次请求就会调用验证服务了. 一定要在UserMvc()之前调用
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
