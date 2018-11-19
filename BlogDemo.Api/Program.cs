using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Blog.Core;
using Blog.Infrastructure.DataBase;
using Serilog;
using Serilog.Events;
namespace BlogDemo.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region Serilog配置
            Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File(Path.Combine("logs", @"log.txt"), rollingInterval: RollingInterval.Day)
                    .CreateLogger(); 
            #endregion

            IWebHost host = CreateWebHostBuilder(args).Build();
            #region 初始化数据库 不是必须的
            //using (var scope = host.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            //    try
            //    {
            //        var myContext = services.GetRequiredService<MyContext>();
            //        MyContextSeed.SeedAsync(myContext, loggerFactory).Wait();
            //    }
            //    catch (Exception e)
            //    {
            //        var logger = loggerFactory.CreateLogger<Program>();
            //        logger.LogError(e,"初始化数据库失败");
            //    }
            //} 
            #endregion
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                //.UseStartup<Startup>();
                //使用不同环境变量的类来配置，启动时使用不同的startup类配置->launchSettings.json->ASPNETCORE_ENVIRONMENT
                .UseStartup(typeof(StartupDevelopment).Assembly.FullName)
                .UseSerilog();  //使用Serilog，会自动覆盖WebHost.CreateDefaultBuilder(args)源生的log
    }
}
