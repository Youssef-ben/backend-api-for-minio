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

        public const string API_BUCKET_ALREADY_EXISTS = "The bucket [{0}] already exists!";
        public const string API_BUCKET_ALREADY_EXISTS_ID = "api.error.bucket.already.exists.id";

        public const string API_ITEM_ALREADY_EXISTS = "The item [{0}] of the bucket [{1}], already exists!";
        public const string API_ITEM_ALREADY_EXISTS_ID = "api.error.bucket.item.already.exists.id";

        // For security reasons, we should return a message as {Can't create the bucket} instead of {Already Exists}.
        public const string API_CANT_CREATE_BUCKET = "Oops, we couldn't create the bucket [{0}], please contact our support team if the problem persists.";
        public const string API_CANT_CREATE_BUCKET_ID = "api.error.bucket.cant.create.id";

        // For security reasons, we should return a message as {Can't update the bucket} instead of {Already Exists}.
        public const string API_CANT_UPDATE_BUCKET = "Oops, we couldn't update the bucket [{0}], please contact our support team if the problem persists.";
        public const string API_CANT_UPDATE_BUCKET_ID = "api.error.bucket.cant.update.id";

        public const string API_BUCKET_NOT_FOUND = "Couldn't find the requested bucket [{0}]. Please retry with a valid name.";
        public const string API_BUCKET_NOT_FOUND_ID = "api.error.bucket.not.found.id";

        public const string API_ITEM_NOT_FOUND = "Couldn't find the requested item [{0}] for the bucket [{1}]. Please retry with a valid name.";
        public const string API_ITEM_NOT_FOUND_ID = "api.error.bucket.item.not.found.id";

        public const string API_FIELDS_INVALID_VALIDATION = "One or more fields are required or invalid!!";
        public const string API_FIELDS_INVALID_VALIDATION_ERROR = "Error Occurred while trying to validate the model state. please refer to the {values} field of this response.";
        public const string API_FIELDS_INVALID_VALIDATION_ID = "api.error.fields.validation.id";

        // Generic messages
        public const string MINIO_API_ERROR = "Sequence contains no elements.";
        public const string LOG_MESSAGE = "Listed all objects in bucket {0}";

        // Global
        public const int DEFAULT_PAGE_LIMITE = 25;
        public const int MAX_BUCKETS_PER_PAGE = 20000;
    }
}
