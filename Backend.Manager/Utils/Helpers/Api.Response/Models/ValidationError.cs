namespace Backend.Minio.Manager.Helpers.Api.Response.Models
{
    using Newtonsoft.Json;

    public class ValidationError
    {
        public ValidationError(string field, string message)
        {
            this.Field = field != string.Empty ? field : null;
            this.Message = message;
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; }

        public string Message { get; }
    }
}
