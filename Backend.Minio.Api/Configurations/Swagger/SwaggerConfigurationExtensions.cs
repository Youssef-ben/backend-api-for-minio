namespace Backend.Minio.Api.Configurations.Swagger
{
    using System;
    using System.IO;
    using System.Reflection;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.PlatformAbstractions;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerUI;

    public static class SwaggerConfigurationExtensions
    {
        public const string LATEST_VERSION = "1.0";

        private static string GetXmlCommentsFilePath
        {
            get
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }

        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection self)
        {
            return self.AddSwaggerGen(
                   options =>
                   {
                       // resolve the IApiVersionDescriptionProvider service
                       // note: we have to build a temporary service provider here because one has not been created yet.
                       var provider = self.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                       // add a swagger document for each discovered API version
                       foreach (var description in provider.ApiVersionDescriptions)
                       {
                           options.SwaggerDoc(description.GroupName, GetInfoForApiVersion(description));
                       }

                       // add a custom operation filter which sets default values
                       options.OperationFilter<SwaggerDefaultValues>();

                       // integrate xml comments
                       options.IncludeXmlComments(GetXmlCommentsFilePath);
                   });
        }

        public static IApplicationBuilder UseCustomeSwagger(this IApplicationBuilder app, IApiVersionDescriptionProvider apiProvider)
        {
            app
                .UseSwagger()
                .UseSwaggerUI(options =>
                    {
                        options.DocExpansion(DocExpansion.None);

                        // build a swagger endpoint for each discovered API version
                        foreach (var description in apiProvider.ApiVersionDescriptions)
                        {
                            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                        }
                    });

            return app;
        }

        private static OpenApiInfo GetInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = $"Backend API for minio - v{description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = "Backend API, is a project used to manage your Object Storage Server (minio).",
                Contact = new OpenApiContact() { Name = "Youssef Benhessou", Email = "ben-ucef@hotmail.fr" },
                License = new OpenApiLicense() { Name = $"© MIT Licence {DateTime.Now.Year}" },
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }
}
