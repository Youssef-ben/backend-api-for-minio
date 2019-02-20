namespace Backend.Tests.Config
{
    using System;
    using Backend.Manager.Utils.Models.ConfigModels;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Nest;

    public static class GetAppsettingsConfigs
    {
        public static IConfiguration LoadConfiguration() => new ConfigurationBuilder()
               .AddJsonFile("appSettings.json", false)
               .AddJsonFile("appSettings.Development.json", true)
               .AddJsonFile("appSettings.Local.json", true)
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
            return new TestConfigurationModel<TManager>()
            {
                Configurations = GetAppsettingsConfigs.LoadConfiguration().GetConfigurationInstance<ElasticsearchConfig>("Elasticsearch"),
                Logger = Mock.Of<ILogger<TManager>>(),
            };
        }

        public static ElasticClient GetElasticSearchClient(this ElasticsearchConfig self)
        {
            var connection = new ConnectionSettings(new Uri($"{self.NodeUri}:{self.Port}"));
            return new ElasticClient(connection);
        }
    }
}
