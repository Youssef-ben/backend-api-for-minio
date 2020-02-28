using Backend.API.Config;
using Backend.Manager.Config;
using Backend.Manager.Utils.Models.ConfigModels;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Backend.API
{
    /// <summary>
    /// Represents the startup process for the application.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">Current Config</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the current configuration.
        /// </summary>
        /// <value>The current application configuration.</value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures services for the application.
        /// </summary>
        /// <param name="services">The collection of services to configure the application with.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddLogging();

            services.AddMvcCore(opt =>
                {
                    opt.Filters.Add(typeof(ValidateModelAttribute));
                })
                .AddApiExplorer()
                .AddFluentValidation(fv =>
                 {
                     fv.RegisterValidatorsFromAssemblyContaining<Startup>();
                     fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                     fv.ImplicitlyValidateChildProperties = true;
                 })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(this.SetJsonConfigurations())
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            services
                .AddApiVersioning(o => o.ReportApiVersions = true)
                .AddVersionedApiExplorer(options => options.GroupNameFormat = "'v'VVV")
                .AddSwaggerConfiguration();

            // Create an instance of IOption for the {Settings} section of the appsettings.
            services.Configure<AppsettingsModel>(this.Configuration.GetSection("Settings"));

            // Register Projects Dependency injection
            ManagerIoc.Register(services, this.Configuration);
        }

        /// <summary>
        /// Configures the application using the provided builder, hosting environment, and logging factory.
        /// </summary>
        /// <param name="app">The current application builder.</param>
        /// <param name="env">The current hosting environment.</param>
        /// <param name="apiVersionProvider">Api versioning provider.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider apiVersionProvider)
        {
            if (env.IsDevelopment() || Program.IsLocal(env))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Custom Configuration
            LocalizationConfig.Configure(app);
            app.UseCustomSwagger(apiVersionProvider);

            app.UseHttpsRedirection();
        }
    }
}
