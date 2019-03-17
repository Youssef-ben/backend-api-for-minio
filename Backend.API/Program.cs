using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Backend.API
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
                 config.SetBasePath(Path.Combine(hostingContext.HostingEnvironment.ContentRootPath, "..", "Settings"));

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
