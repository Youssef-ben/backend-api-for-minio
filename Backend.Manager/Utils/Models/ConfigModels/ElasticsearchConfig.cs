namespace Backend.Manager.Utils.Models.ConfigModels
{
    public class ElasticsearchConfig
    {
        public string NodeUri { get; set; }

        public int Port { get; set; }

        public string DefaultIndex { get; set; }

        public string PipelineIndex { get; set; }

        public string PipelineDescription { get; set; }

        public bool PasswordRequired { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
