using System;
using System.Runtime.CompilerServices;
using Backend.Fileupload.API.Helpers.Errors.Extension;

namespace Backend.Fileupload.API.Helpers.Errors.CustomErrors
{
    public class ApplicationApiException : BaseCustomError
    {
        public ApplicationApiException()
        {
        }

        public ApplicationApiException(string message)
            : base(message)
        {
        }

        public ApplicationApiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ApplicationApiException(string method, ErrorTypes errorType, object values = null, Exception innerException = null)
            : base(method, errorType, values, innerException)
        {
        }

        public ApplicationApiException(BackendLayersNames layer, ErrorTypes errorType, object values = null, Exception innerException = null, [CallerMemberName] string methodName = null)
            : base($"{layer.GetDescription()}{methodName}", errorType, values, innerException)
        {
        }
    }
}
