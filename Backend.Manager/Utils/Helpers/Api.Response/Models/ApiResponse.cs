namespace Backend.Minio.Manager.Helpers.Api.Response.Models
{
    using Newtonsoft.Json;

    public class ApiResponse<T> : ErrorResponse
        where T : class
    {
        public ApiResponse()
        {
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T Result { get; set; }
    }
}
