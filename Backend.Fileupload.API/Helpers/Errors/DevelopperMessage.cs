using Newtonsoft.Json;

namespace Backend.Fileupload.API.Helpers.Errors
{
    public class DevelopperMessage
    {
        /// <summary>
        /// Gets or sets the method that throw the error.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets The Developper Error.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets The Values that generated the Error.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Values { get; set; }

        /// <summary>
        /// Gets or sets additional informations about the error.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string InnerException { get; set; }

        /// <summary>
        /// Gets or sets additional informations about the error.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string StackTrace { get; set; }
    }
}
