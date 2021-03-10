using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Datasilk.Core.Extensions;

namespace CoreTemplate
{
    public class Startup
    {
        private static IConfigurationRoot config;
        private List<Assembly> assemblies = new List<Assembly> { Assembly.GetCallingAssembly() };

        public virtual void ConfigureServices(IServiceCollection services)
        {
            //set up Server-side memory cache
            services.AddDistributedMemoryCache();
            services.AddMemoryCache();

            //configure request form options
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });

            //add session
            services.AddSession();

            //add health checks
            services.AddHealthChecks();
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
        {
            App.IsDocker = System.Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

            //get environment based on application build
            switch (env.EnvironmentName.ToLower())
            {
                case "production":
                    App.Environment = Environment.production;
                    break;
                case "staging":
                    App.Environment = Environment.staging;
                    break;
                default:
                    App.Environment = Environment.development;
                    break;
            }

            //load application-wide cache
            var configFile = "config" +
                (App.IsDocker ? ".docker" : "") +
                (App.Environment == Environment.production ? ".prod" : "") + ".json";

            config = new ConfigurationBuilder()
                .AddJsonFile(App.MapPath(configFile))
                .AddEnvironmentVariables().Build();

            //configure Server defaults
            App.Host = config.GetSection("hostUri").Value ?? "http://localhost:7000";

            //configure Server database connection strings
            Query.Sql.ConnectionString = config.GetSection("sql:" + config.GetSection("sql:Active").Value).Value;

            //configure Server security
            App.BcryptWorkfactor = int.Parse(config.GetSection("encryption:bcrypt_work_factor").Value);
            App.Salt = config.GetSection("encryption:salt").Value;

            //configure cookie-based authentication
            var expires = !string.IsNullOrWhiteSpace(config.GetSection("Session:Expires").Value) ? int.Parse(config.GetSection("Session:Expires").Value) : 60;

            //use session
            var sessionOpts = new SessionOptions();
            sessionOpts.Cookie.Name = "Datasilk";
            sessionOpts.IdleTimeout = TimeSpan.FromMinutes(expires);

            app.UseSession(sessionOpts);

            //handle static files
            var provider = new FileExtensionContentTypeProvider();

            // Add static file mappings
            provider.Mappings[".svg"] = "image/svg+xml";
            var options = new StaticFileOptions
            {
                ContentTypeProvider = provider
            };
            app.UseStaticFiles(options);

            //exception handling
            if (App.Environment == Environment.development)
            {
                app.UseDeveloperExceptionPage(new DeveloperExceptionPageOptions
                {
                    SourceCodeLineCount = 10
                });
            }
            else
            {
                //use HTTPS
                app.UseHsts();
                app.UseHttpsRedirection();

                //use health checks
                app.UseHealthChecks("/health");
            }

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //set up database
            App.HasAdmin = Query.Users.HasAdmin();

            //set up Saber language support
            App.Languages = new Dictionary<string, string>();
            App.Languages.Add("en", "English"); //english should be the default language

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


            //run Datasilk Core MVC Middleware
            app.UseDatasilkMvc(new MvcOptions()
            {
                IgnoreRequestBodySize = true,
                ServicePaths = new string[] { "api", "gmail" },
                Routes = new Routes()
            });

            //handle missing static files
            app.Use(async (Context, next) => {
                if (Context.Response.StatusCode == 404 && Context.Request.Path.Value.Contains("/content/"))
                {
                    //missing static files that belong to Saber webpages that haven't been saved yet, 
                    //or the user saved the html file using the Editor UI, but haven't saved the less or js files
                    var extension = GetFileExtension(Context.Request.Path.Value).ToLower();
                    switch (extension)
                    {
                        case "js":
                            Context.Response.StatusCode = 200;
                            await Context.Response.WriteAsync("(function(){\n\n})();");
                            break;
                        case "css":
                            Context.Response.StatusCode = 200;
                            await Context.Response.WriteAsync("");
                            break;
                    }
                }
            });

            Console.WriteLine("Running Saber Server in " + App.Environment.ToString() + " environment");
        }

        private string GetFileExtension(string filename)
        {
            for (int x = filename.Length - 1; x >= 0; x += -1)
            {
                if (filename.Substring(x, 1) == ".")
                {
                    return filename.Substring(x + 1);
                }
            }

            return "";
        }
    }
}
