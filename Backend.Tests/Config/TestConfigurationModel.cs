namespace Backend.Tests.Config
{
    using Backend.Manager.Utils.Models.ConfigModels;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class TestConfigurationModel<TManager>
        where TManager : class
    {

        public IOptions<AppsettingsModel> Configurations { get; set; }

        public ILogger<TManager> Logger { get; set; }
    }
}
