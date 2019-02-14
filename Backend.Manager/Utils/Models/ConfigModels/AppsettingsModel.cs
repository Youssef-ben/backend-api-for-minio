namespace Backend.Manager.Utils.Models.ConfigModels
{
    public class AppsettingsModel
    {
        public ApiVersion Api { get; set; }

        public ElasticsearchConfig Elasticsearch { get; set; }
    }
}
