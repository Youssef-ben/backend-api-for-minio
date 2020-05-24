using System.IO;
using Newtonsoft.Json;

namespace Backend.Minio.Manager.Utils.Models
{
    public class MinioFile
    {
        [JsonIgnore]
        public MemoryStream StreamContent { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }
    }
}
