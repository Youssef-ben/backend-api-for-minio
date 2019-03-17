using System;
using Backend.Manager.Utils.Models.ConfigModels;
using Microsoft.Extensions.Configuration;
using Minio;
using Nest;

namespace Backend.Manager.Utils.Helpers.ConfigExtensions
{
    public static class ConfigExtensions
    {
        public static TClass GetConfigurationInstance<TClass>(this IConfiguration self, string section)
            where TClass : class, new()
        {
            var instance = new TClass();
            self.Bind(section, instance);
            return instance;
        }

        public static ElasticClient GetElasticSearchClient(this BackendConfiguration self)
        {
            var connection = new ConnectionSettings(new Uri($"{self.NodeUri}:{self.Port}"));
            return new ElasticClient(connection);
        }

        public static MinioClient GetMinioClient(this BackendConfiguration self)
        {
            return new MinioClient($"{self.NodeUri}:{self.Port}", self.Username, self.Password);
        }
    }
}
