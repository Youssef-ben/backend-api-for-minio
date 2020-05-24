namespace Backend.Minio.Api.Configurations
{
    using Backend.Minio.Manager.Utils.Helpers.ConfigExtensions;
    using Backend.Minio.Manager.Utils.Models.ConfigModels;
    using FluentValidation.AspNetCore;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Versioning;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ApiConfigurationsExtensions
    {
        public static IMvcBuilder AddFluentValidatorServices(this IServiceCollection self)
        {
            return self.AddControllersWithViews(opt =>
                {
                    // Add Validator.
                    opt.Filters.Add(typeof(ValidateModelAttribute));
                })
               .AddFluentValidation(x =>
               {
                   x.RegisterValidatorsFromAssemblyContaining<SettingsModel>();

                   /*
                    * Disable the Default MVC validator.
                    * This will allow adding DataAnnotations to models for Swagger to flag the required fields
                    * without getting a double validation errors.
                    */
                   x.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                   x.ImplicitlyValidateChildProperties = true;
               });
        }

        public static IServiceCollection AddApiVersioningServices(this IServiceCollection self)
        {
            self.AddApiVersioning(o =>
                {
                    o.UseApiBehavior = false;
                    o.ReportApiVersions = true;
                    o.DefaultApiVersion = new ApiVersion(1, 0);
                    o.AssumeDefaultVersionWhenUnspecified = true;
                    o.ApiVersionReader = new UrlSegmentApiVersionReader();
                })
                .AddVersionedApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";

                    // Note: this option is only necessary when versioning by url segment.
                    // The SubstitutionFormat can also be used to control the format of the API version in route templates.
                    options.SubstituteApiVersionInUrl = true;
                });

            return self;
        }

        public static IServiceCollection AddMinioClientServices(this IServiceCollection self, IConfiguration configuration)
        {
            // Get Configuration
            var minioConfig = configuration.GetSettings<SettingsModel>().Minio;

            // Set the Client to be a singleton, since we need only one connection instance.
            return self.AddSingleton(minioConfig.GetMinioClient());
        }

        public static IServiceCollection AddSettingsServices(this IServiceCollection self, IConfiguration configuration)
        {
            return self
                .Configure<SettingsModel>(configuration.GetSection("Settings"))
                .Configure<MinioSettings>(configuration.GetSection("Settings")); ;
        }
    }
}
