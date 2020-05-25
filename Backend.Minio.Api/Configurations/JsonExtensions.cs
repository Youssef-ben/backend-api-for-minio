namespace Backend.API.Config
{
    using System;
    using Backend.Minio.Api;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public static class JsonExtensions
    {
        public static Action<MvcNewtonsoftJsonOptions> GetJsonConfigurations(this Startup self)
        {
            if (self is null)
            {
                throw new ArgumentNullException("The parameter {Startup} is required !");
            }

            return options =>
            {
                var resolver = new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() };

                options.SerializerSettings.ContractResolver = resolver;
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                options.SerializerSettings.Formatting = Formatting.Indented;
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            };
        }
    }
}
