namespace Backend.Minio.Manager.Models
{
    using System.IO;
    using Newtonsoft.Json;

    public class MinioItem
    {
        [JsonIgnore]
        public MemoryStream StreamContent { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }
    }
}
