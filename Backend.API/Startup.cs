using Backend.API.Config;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
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
            
            services.AddMvc(opt =>
            {
                opt.Filters.Add(typeof(ValidateModelAttribute));
            })
                 .AddFluentValidation(fv =>
                 {
                     fv.RegisterValidatorsFromAssemblyContaining<Startup>();
                     fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                     fv.ImplicitlyValidateChildProperties = true;
                 })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(JsonConfig.Configure())
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
            // note: the specified format code will format the version as "'v'major[.minor][-status]"
            services.AddMvcCore()
                .AddVersionedApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";

                    // Note: this option is only necessary when versioning by url segment. the Substitution Format
                    // can also be used to control the format of the API version in route templates.
                    options.SubstituteApiVersionInUrl = true;
                });

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
            });

            // Register Projects Dependency injection
            SwaggerConfig.Register(services);
        }

        /// <summary>
        /// Configures the application using the provided builder, hosting environment, and logging factory.
        /// </summary>
        /// <param name="app">The current application builder.</param>
        /// <param name="env">The current hosting environment.</param>
        /// <param name="loggerFactory">The logging factory used for instrumentation.</param>
        /// <param name="provider">The API version descriptor provider used to enumerate defined API versions.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApiVersionDescriptionProvider provider)
        {
            loggerFactory.AddConsole(this.Configuration.GetSection("Logging"));
            loggerFactory.AddFile(this.Configuration.GetSection("Logging").GetValue<string>("PathFormat"), isJson: true);

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
            SwaggerConfig.Configure(app, provider);

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
