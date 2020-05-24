namespace Backend.Minio.Api
{
    using Backend.API.Config;
    using Backend.Minio.Api.Configurations;
    using Backend.Minio.Api.Configurations.Swagger;
    using Backend.Minio.Manager.Ioc;
    using Facilis.Api.Helpers.Exception;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add Global services.
            services
                .AddFluentValidatorServices()
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .AddNewtonsoftJson(this.GetJsonConfigurations());

            // Add Custom Services.
            services
                .AddApiVersioningServices()
                .AddResponseCompression()
                .AddSettingsServices(this.Configuration)
                .AddSwaggerConfiguration()
                .AddMinioClientServices(this.Configuration)
                .AddManagerServices();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider apiProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .ConfigureExceptionHandler()
                .UseRouting()
                .UseAuthorization()
                .UseCustomeSwagger(apiProvider)
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }
    }
}
