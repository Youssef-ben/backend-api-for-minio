namespace Backend.Manager.Utils.Models.ConfigModels
{
    public class AppsettingsModel
    {
        public ApiVersion Api { get; set; }

        public BackendConfiguration Minio { get; set; }

        public BackendConfiguration Elasticsearch { get; set; }
    }
}
