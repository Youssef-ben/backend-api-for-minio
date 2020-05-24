namespace Backend.Minio.Api
{
    using System.IO;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Serilog;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, configBuilder) =>
            {
                configBuilder
                    .SetBasePath(Path.Combine(hostingContext.HostingEnvironment.ContentRootPath, "..", "Settings"))
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)

                    // You can set ASPNETCORE_ENVIRONMENT = Local, Development, Staging or Production
                    .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args);
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                .UseStartup<Startup>()
                .UseKestrel((hostingContext, options) =>
                {
                    // Increse the request limit to 2GO - For the Files upload
                    options.Limits.MaxRequestBodySize = 2147483648;
                });
            })
            .UseSerilog((hostingContext, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom
                    .Configuration(hostingContext.Configuration)
                    .Enrich
                    .FromLogContext()
                    .WriteTo.Console();
            });
    }
}
