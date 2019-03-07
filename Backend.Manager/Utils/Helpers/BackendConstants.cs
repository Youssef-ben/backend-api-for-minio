namespace Backend.Manager.Helpers
{
    public class BackendConstants
    {
        #region " [GENERAL - ERRORS MESSAGES] "

        public const string CreateParameterRequired = "Create method recieved a null parameter.";

        public const string UpdateParameterRequired = "Update method recieved a null parameter.";

        public const string DeleteParameterRequired = "Delete method recieved a null parameter.";

        public const string GetParameterRequired = "Get method recieved a null parameter.";

        public const string ParametersRequired = "You must specify the required parameters.";

        public const string AlreadyExists = "An object with the same values already exists.";

        public const string UnexpectedError = "An error occurred while saving/updating the object.";

        public const string NotFound = "The object you're looking for doesn't exists.";

        public const string InvalidObjectValues = "Invalid object value(s).";

        public const string AtleastOneValueNeeded = "You must enter at least one value.";

        #endregion

        #region " [ELASTICSEARCH - ERRORS MESSAGES] "

        public const string ErrorWhileCreatingEsIndex = "Error While Creating the Elasticsearch base index.";

        public const string ErrorWhileCreatingEsIngestPipline = "Error While Creating the Elasticsearch Ingest pipeline.";

        public const string ErrorWhileIndexingTheDocument = "Error While Indexing the document in elasticsearch.";

        public const string ErrorWhileDeletingTheDocument = "Error While Deleting the document in elasticsearch.";

        public const string ErrorWhileUpdatingTheDocument = "Error While Updating the document in elasticsearch.";

        public const string ErrorWhileSearchingForValues = "Error While searching for the values.";

        public const string ErrorWhileDeletingTheIndex = "Error While Deleting the Specified Index in elasticsearch.";

        #endregion

        #region " [MINIO SECTION] "

        public const string ErrorWhileCreatingMinioBucket = "Error While Creating the Minio Bucket.";

        public const string ErrorMinioBucketDoesntExists = "The bucket you specified doesn't exists. please specify a valid name.";

        #endregion

        #region " [LOGGING MESSAGES] "

        /// <summary>
        /// Value : {"Started the request process for the user[{0}]. Object[{1}]"}.
        /// - {0} : The User ID.
        /// - {1} : The ID of the object to process.
        /// </summary>
        public const string ApiStartedTheRequestProcess = "Started the request process for the user[{0}]. Object[{1}]";

        /// <summary>
        /// Value : {"Returning the result to The user[{0}]."}.
        /// - {0} : The User ID.
        /// </summary>
        public const string ApiReturningTheResult = "Returning the result to The user[{0}].";

        /// <summary>
        /// Value : {"Recieved an invalid value from the user[{0}]. Object[{1}]."}.
        /// - {0} : The User ID.
        /// - {1} : The Value.
        /// </summary>
        public const string ApiRecievedInvalidValue = "Recieved an invalid value from the user[{0}]. Object[{1}].";

        /// <summary>
        /// Value : {"Started the manager action [{0}] for the user[{1}]. Object[{2}]."}.
        /// - {0} : Method Name.
        /// - {1} : The ID of the user.
        /// - {2} : The ID of the object to process.
        /// </summary>
        public const string ManagerStartedTheAction = "Started the manager action [{0}] for the user[{1}]. Object[{2}].";

        /// <summary>
        /// Value : {"Completing the manager action [{0}] for the user[{1}]. Object[{2}]."}.
        /// - {0} : Method Name.
        /// - {1} : The ID of the user.
        /// - {2} : The ID of the object to process.
        /// </summary>
        public const string ManagerCompletingTheAction = "Completing the manager action [{0}] for the user[{1}]. Object[{2}].";

        #endregion
    }
}
