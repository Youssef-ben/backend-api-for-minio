using System.ComponentModel;

namespace Backend.Manager.Helpers.Errors
{
    public enum ErrorTypes : int
    {
        /// <summary>
        /// Used in the Manager, Repository layers
        /// Value : CREATE ERROR.
        /// </summary>
        [Description(BackendConstants.CreateParameterRequired)]
        CREATE,

        /// <summary>
        /// Used in the Manager, Repository layers
        /// Value : UPDATE ERROR.
        /// </summary>
        [Description(BackendConstants.UpdateParameterRequired)]
        UPDATE,

        /// <summary>
        /// Used in the Manager, Repository layers
        /// Value : DELETE ERROR.
        /// </summary>
        [Description(BackendConstants.DeleteParameterRequired)]
        DELETE,

        /// <summary>
        /// Used in the Manager, Repository layers
        /// Value : FETCH ERROR.
        /// </summary>
        [Description(BackendConstants.GetParameterRequired)]
        FETCH,

        /// <summary>
        /// Used in the Manager, Repository layers
        /// Value : SEARCH ERROR.
        /// </summary>
        [Description(BackendConstants.GetParameterRequired)]
        SEARCH,

        /// <summary>
        /// Used in the API layers
        /// Value : EMPTY VALUES
        /// </summary>
        [Description(BackendConstants.ParametersRequired)]
        EMPTY_VALUES,

        /// <summary>
        /// Values : UNEXPECTEDERROR ERRO.
        /// </summary>
        [Description(BackendConstants.UnexpectedError)]
        UNEXPECTED_ERROR,

        /// <summary>
        /// Values : ALREADY EXISTS.
        /// </summary>
        [Description(BackendConstants.AlreadyExists)]
        ALREADY_EXISTS,

        /// <summary>
        /// Values : NOT FOUND.
        /// </summary>
        [Description(BackendConstants.NotFound)]
        NOT_FOUND,

        /// <summary>
        /// Values : INVALID OBJECT VALUES.
        /// </summary>
        [Description(BackendConstants.InvalidObjectValues)]
        INVALID_OBJECT_VALUES,

        /// <summary>
        /// Values : Error while Creating the elasticseach base Index.
        /// </summary>
        [Description(BackendConstants.ErrorWhileCreatingEsIndex)]
        ERROR_WHILE_CREATING_ES_INDEX,

        /// <summary>
        /// Values : Error while Creating the elasticseach ingest pipline
        /// </summary>
        [Description(BackendConstants.ErrorWhileCreatingEsIngestPipline)]
        ERROR_WHILE_CREATING_ES_INGEST_PIPLINE,

        /// <summary>
        /// Values : Error Indexing the document.
        /// </summary>
        [Description(BackendConstants.ErrorWhileIndexingTheDocument)]
        ERROR_WHILE_INDEXING_THE_DOCUMENT,

        /// <summary>
        /// Values : Error Deleting the document.
        /// </summary>
        [Description(BackendConstants.ErrorWhileDeletingTheDocument)]
        ERROR_WHILE_DELETING_THE_DOCUMENT,

        /// <summary>
        /// Values : Error Updating the document.
        /// </summary>
        [Description(BackendConstants.ErrorWhileUpdatingTheDocument)]
        ERROR_WHILE_UPDATING_THE_DOCUMENT,

        /// <summary>
        /// Values : Error While searching for the values.
        /// </summary>
        [Description(BackendConstants.ErrorWhileSearchingForValues)]
        ERROR_WHILE_SEARCHING_FOR_VALUES,
    }
}
