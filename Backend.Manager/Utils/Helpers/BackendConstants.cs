namespace Backend.Manager.Helpers
{
    public class BackendConstants
    {
        #region " [ERRORS MESSAGES] "

        /// <summary>
        /// Value : {Create method recieved a null parameter.}.
        /// </summary>
        public const string CreateParameterRequired = "Create method recieved a null parameter.";

        /// <summary>
        /// Value : {Update method recieved a null parameter.}.
        /// </summary>
        public const string UpdateParameterRequired = "Update method recieved a null parameter.";

        /// <summary>
        /// Value : {Delete method recieved a null parameter.}.
        /// </summary>
        public const string DeleteParameterRequired = "Delete method recieved a null parameter.";

        /// <summary>
        /// Value : {Get method recieved a null parameter.}.
        /// </summary>
        public const string GetParameterRequired = "Get method recieved a null parameter.";

        /// <summary>
        /// Value : {You must specify the required parameters.}.
        /// </summary>
        public const string ParametersRequired = "You must specify the required parameters.";

        /// <summary>
        /// Value : {An object with the same values already exists.}.
        /// </summary>
        public const string AlreadyExists = "An object with the same values already exists.";

        /// <summary>
        /// Value : {An error occured while saving/updating the object.}.
        /// </summary>
        public const string UnexpectedError = "An error occurred while saving/updating the object.";

        /// <summary>
        /// Value : {The object you're looking for doesn't exists.}.
        /// </summary>
        public const string NotFound = "The object you're looking for doesn't exists.";

        /// <summary>
        /// Value : {Invalid object value(s).}.
        /// </summary>
        public const string InvalidObjectValues = "Invalid object value(s).";

        /// <summary>
        /// Value : {You must enter at least one value.}.
        /// </summary>
        public const string AtleastOneValueNeeded = "You must enter at least one value.";

        /// <summary>
        /// Value : {Error While Creating the Elasticsearch Ingest pipeline.}.
        /// </summary>
        public const string ErrorWhileCreatingEsIndex = "Error While Creating the Elasticsearch base index.";

        /// <summary>
        /// Value : {Error While Creating the Elasticsearch Ingest pipeline.}.
        /// </summary>
        public const string ErrorWhileCreatingEsIngestPipline = "Error While Creating the Elasticsearch Ingest pipeline.";

        /// <summary>
        /// Value : {Error While Indexing the document in elasticsearch.}.
        /// </summary>
        public const string ErrorWhileIndexingTheDocument = "Error While Indexing the document in elasticsearch.";

        /// <summary>
        /// Value : {Error While Deleting the document in elasticsearch.}.
        /// </summary>
        public const string ErrorWhileDeletingTheDocument = "Error While Deleting the document in elasticsearch.";

        /// <summary>
        /// Value : {Error While Deleting the document in elasticsearch.}.
        /// </summary>
        public const string ErrorWhileUpdatingTheDocument = "Error While Updating the document in elasticsearch.";

        /// <summary>
        /// Value : {Error While searching for the values.}.
        /// </summary>
        public const string ErrorWhileSearchingForValues = "Error While searching for the values.";

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
