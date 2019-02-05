using System;
using System.Runtime.CompilerServices;
using Backend.Fileupload.API.Helpers.Errors.Extension;

namespace Backend.Fileupload.API.Helpers.Errors.CustomErrors
{
    public class ApplicationRepositoryException : BaseCustomError
    {
        public ApplicationRepositoryException()
        {
        }

        public ApplicationRepositoryException(string message)
            : base(message)
        {
        }

        public ApplicationRepositoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ApplicationRepositoryException(BackendLayersNames layer, ErrorTypes errorType, object values = null, Exception innerException = null, [CallerMemberName] string methodName = null)
            : base($"{layer.GetDescription()}{methodName}", errorType, values, innerException)
        {
        }
    }
}
