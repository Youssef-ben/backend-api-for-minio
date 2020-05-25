namespace Backend.Minio.Manager.Helpers.Api.Response.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Class Defining the fields that will be returned to the developper.
    /// </summary>
    public class ErrorResponse
    {
        public ErrorResponse()
        {
            this.DeveloperMessage = new DevelopperMessage();
        }

        public string Message { get; set; }

        public string MessageID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DevelopperMessage DeveloperMessage { get; set; }
    }
}
