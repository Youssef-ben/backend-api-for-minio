namespace Backend.Minio.Manager.Helpers.Api.Response.Models
{
    using Newtonsoft.Json;

    public class DevelopperMessage
    {
        public string Method { get; set; }

        public string Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Values { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string InnerException { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string StackTrace { get; set; }
    }
}
