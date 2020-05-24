using Newtonsoft.Json;

namespace Backend.Minio.Api.Models
{
    public class BucketDto
    {
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string NewName { get; set; }
    }
}
