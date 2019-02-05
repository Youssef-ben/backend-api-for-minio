using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Backend.Fileupload.API
{
    public class Program
    {
        private const string LOCAL = "Local";
        private const string DEVELOPMENT = "Development";
        private const string STAGING = "Staging";
        private const string PRODUCTION = "Production";

        public static void Main(string[] args)
        {
            var webHost = CreateWebHostBuilder(args)
                .UseKestrel((hostingContext, options) =>
                {
                    // Increse the request limit to 2GO - For the Files upload
                    options.Limits.MaxRequestBodySize = 2147483648;
                });

            webHost.Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
             WebHost.CreateDefaultBuilder(args)
             .ConfigureAppConfiguration((hostingContext, config) =>
             {
                 var rootContent = IsLocal(hostingContext.HostingEnvironment) ? "\\..\\Settings\\" : string.Empty;
                 config.SetBasePath($"{hostingContext.HostingEnvironment.ContentRootPath}{rootContent}");

                 config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                 // You can set ASPNETCORE_ENVIRONMENT = Local, Development, Staging or Production
                 config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: false, reloadOnChange: true);
                 config.AddEnvironmentVariables();
                 config.AddCommandLine(args);
             })
            .UseStartup<Startup>();

        public static bool IsLocal(IHostingEnvironment env)
        {
            return LOCAL.Equals(env.EnvironmentName);
        }
    }
}
