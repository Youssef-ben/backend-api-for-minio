namespace Backend.Tests.Config
{
    using Backend.Manager.Utils.Models.ConfigModels;
    using Microsoft.Extensions.Logging;

    public class TestConfigurationModel<TManager>
        where TManager : class
    {

        public ElasticsearchConfig Configurations { get; set; }

        public ILogger<TManager> Logger { get; set; }
    }
}
