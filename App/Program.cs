﻿using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoreTemplate
{
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                //.UseKestrel(
                //    options =>
                //    {
                //        options.Limits.MaxRequestBodySize = null;
                //        //options.ListenAnyIP(80); //for docker
                //    }
                //)
                .UseStartup<Startup>();
            })
            .ConfigureServices(services =>
            {
                services.AddLogging(cfg =>
                    cfg.AddConsole(opts =>
                    {
                        opts.IncludeScopes = false;
                    }));
            });


        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
    }
}