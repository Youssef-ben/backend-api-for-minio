namespace Backend.Minio.Manager.Helpers
{
    public class Constants
    {
        public const string API_REPONSE_SUCCESS = "Request succeeded";
        public const string API_REPONSE_SUCCESS_ID = "api.reponse.success.id";

        public const string API_REPONSE_FAILED = "Request Failed!";
        public const string API_REPONSE_FAILED_ID = "api.reponse.Failed.id";

        public const string API_UNEXPECTED_ERROR_MESSAGE = "Sorry, An unexpected error occurred while trying to handle your request, please contact the support team for more information !";
        public const string API_UNEXPECTED_ERROR_MESSAGE_ID = "api.error.internal.error.id";

        public const string API_BAD_REQUEST_ERROR_MESSAGE_ID = "api.error.bad.request.id";

        public const string API_ALREADY_EXISTS = "The bucket [{0}] already exists !!";
        public const string API_ALREADY_EXISTS_ID = "api.error.bucket.already.exists.id";

        public const string API_NOT_FOUND = "Couldn't find the requested bucket [{0}]. Please retry with a valid name.";
        public const string API_NOT_FOUND_ID = "api.error.bucket.not.found.id";

        public const string API_INVALID_FIELDS_VALIDATION = "One or more fields are required or invalid!!";
        public const string API_INVALID_FIELDS_VALIDATION_ERROR = "Error Occurred while trying to validate the model state. please refer to the {values} field of this response.";
        public const string API_INVALID_FIELDS_VALIDATION_ID = "api.error.fields.validation.id";

        // Generic messages
        public const string MINIO_API_ERROR = "Sequence contains no elements.";
        public const string LOG_MESSAGE = "Listed all objects in bucket {0}";
    }
}
