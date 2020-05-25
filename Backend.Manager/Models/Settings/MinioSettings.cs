namespace Backend.Minio.Manager.Models.Settings
{
    public class MinioSettings
    {
        public string NodeUri { get; set; }

        public string DefaultIndex { get; set; }

        public string PipelineIndex { get; set; }

        public string PipelineDescription { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
