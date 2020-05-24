namespace Backend.Minio.Manager.Utils.Helpers.Api.Response.Models
{
    using Newtonsoft.Json;

    public class ExtrasDetails
    {
        public string Manager { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Details { get; set; }
    }
}
