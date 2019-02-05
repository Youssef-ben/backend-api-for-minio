using System.ComponentModel;

namespace Backend.Fileupload.API.Helpers.Errors
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
    }
}
