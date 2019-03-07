namespace Backend.Tests.Config
{
    using System;
    using Backend.Manager.Utils.Models.ConfigModels;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Minio;
    using Moq;
    using Nest;

    public static class GetAppsettingsConfigs
    {
        public static IConfiguration LoadConfiguration() => new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", false)
               .AddJsonFile("appsettings.Development.json", true)
               .AddJsonFile("appsettings.Local.json", true)
               .Build();

        public static TClass GetConfigurationInstance<TClass>(this IConfiguration self, string section)
            where TClass : class, new()
        {
            var instance = new TClass();
            self.Bind(section, instance);
            return instance;
        }

        public static TestConfigurationModel<TManager> GetConfiguration<TManager>()
            where TManager : class
        {
            var config = GetAppsettingsConfigs.LoadConfiguration().GetConfigurationInstance<AppsettingsModel>("Settings");
            return new TestConfigurationModel<TManager>()
            {
                Configurations = Options.Create(config),
                Logger = Mock.Of<ILogger<TManager>>(),
            };
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
