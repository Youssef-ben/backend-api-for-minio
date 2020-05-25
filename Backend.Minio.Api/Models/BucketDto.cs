namespace Backend.Minio.Api.Models
{
    using Newtonsoft.Json;

    public class BucketDto
    {
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string NewName { get; set; }
    }
}
