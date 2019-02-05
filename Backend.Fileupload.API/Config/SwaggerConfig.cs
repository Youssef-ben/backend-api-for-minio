using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;

namespace Backend.Fileupload.API.Config
{
    /// <summary>
    /// Used to configure the Swagger API at the startup project.
    /// </summary>
    public static class SwaggerConfig
    {
        public static string XmlCommentsFilePath
        {
            get
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }

        public static void Register(IServiceCollection services)
        {
            services.AddSwaggerGen(
                options =>
                {
                    // resolve the IApiVersionDescriptionProvider service
                    // note: that we have to build a temporary service provider here because one has not been created yet
                    var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                    // add a swagger document for each discovered API version
                    // note: you might choose to skip or document deprecated API versions differently
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                    }

                    // add a custom operation filter which sets default values
                    options.OperationFilter<SwaggerDefaultValues>();

                    // integrate xml comments
                    options.IncludeXmlComments(XmlCommentsFilePath);
                });
        }

        public static void Configure(IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger();

            app.UseSwaggerUI(
                options =>
                {
                    // build a swagger endpoint for each discovered API version
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }
                });
        }

        public static Info CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new Info()
            {
                Title = $"Backend Fileupload v{description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = "Backend Fileupload API, is a single access point to manage your Object Storage Server (minio).",
                Contact = new Contact() { Name = "Youssef Benhessou", Email = "ben-ucef@hotmail.fr" },
                License = new License() { Name = $"© MIT Licence {DateTime.Now.Year}" },
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }
}
