using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Backend.API.Config
{
    public static class JsonConfig
    {
        public static Action<MvcJsonOptions> Configure()
        {
            return jsonOptions =>
            {
                jsonOptions.SerializerSettings.ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy(),
                };

                // Ignore the Looping reference when including an entirty inside an entity.
                jsonOptions.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            };
        }
    }
}
