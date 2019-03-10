namespace Backend.Tests.Config
{
    using Backend.Manager.Utils.Helpers.ConfigExtensions;
    using Backend.Manager.Utils.Models.ConfigModels;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;

    public static class GetAppsettingsConfigs
    {
        public static IConfiguration LoadConfiguration() => new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", false)
               .AddJsonFile("appsettings.Development.json", true)
               .AddJsonFile("appsettings.Local.json", true)
               .Build();

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
    }
}
