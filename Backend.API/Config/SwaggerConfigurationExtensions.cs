namespace Backend.API.Config
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
        private static string GetXmlCommentsFilePath
        {
            get
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }

        // JIRA Ticket CORE-271
        // TODO : Convert this to IPostConfigureOption. See https://andrewlock.net/avoiding-startup-service-injection-in-asp-net-core-3/
        // https://facilisglobal.atlassian.net/browse/CORE-271

        /// <summary>
        /// Configure and add Swagger to the Dependency Injection. Also support the API versioning.
        /// </summary>
        /// <param name="self">The Services Collection..</param>
        /// <returns>Service Collection.</returns>
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

        /// <summary>
        /// Set the application to use swagger and generate the UI for it when requested.
        /// It's configured only if the {useSwagger} option is true.
        /// </summary>
        /// <param name="app">The application Builder.</param>
        /// <param name="apiProvider">The Api version provider.</param>
        /// <returns>Application Builder.</returns>
        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app, IApiVersionDescriptionProvider apiProvider)
        {
            app.UseSwagger();

            app.UseSwaggerUI(
                options =>
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
