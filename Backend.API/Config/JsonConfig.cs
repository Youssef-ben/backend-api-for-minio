using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Backend.API.Config
{
    public static class JsonConfig
    {
        public static Action<MvcNewtonsoftJsonOptions> SetJsonConfigurations(this Startup self)
        {
            if (self is null)
            {
                throw new ArgumentNullException($"The {nameof(Startup)} reuqired!");
            }

            return options =>
            {
                options.SerializerSettings.ContractResolver = new SnakeCaseContractResolver();
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                options.SerializerSettings.Formatting = Formatting.Indented;
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            };
        }
    }
}
