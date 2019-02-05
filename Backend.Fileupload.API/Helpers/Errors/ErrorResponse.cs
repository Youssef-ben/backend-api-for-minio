using System.Collections.Generic;
using Newtonsoft.Json;

namespace Backend.Fileupload.API.Helpers.Errors
{
    /// <summary>
    /// Class Defining the fields that will be returned to the developper.
    /// </summary>
    public class ErrorResponse
    {
        public ErrorResponse()
        {
            this.DeveloperMessage = new DevelopperMessage();
        }

        /// <summary>
        /// Gets or sets the object to an {int : 0/1}.
        /// {0: No Error, 1: Error}.
        /// </summary>
        public int Error { get; set; }

        /// <summary>
        /// Gets or sets the Status Code of the Error.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets the Mesage error destined for the user.
        /// </summary>
        public string UserMessage { get; set; }

        /// <summary>
        /// Gets or sets the message destined for the developer.
        /// </summary>
        public DevelopperMessage DeveloperMessage { get; set; }

        /// <summary>
        /// Gets or sets the models validation errors.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<ValidationError> ValidationErrors { get; set; }
    }
}
