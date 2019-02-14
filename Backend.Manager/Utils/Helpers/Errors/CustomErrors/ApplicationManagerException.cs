using System;
using System.Runtime.CompilerServices;
using Backend.Manager.Helpers.Extension;

namespace Backend.Manager.Helpers.Errors.CustomErrors
{
    public class ApplicationManagerException : BaseCustomError
    {
        public ApplicationManagerException()
        {
        }

        public ApplicationManagerException(string message)
            : base(message)
        {
        }

        public ApplicationManagerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ApplicationManagerException(BackendLayersNames layer, ErrorTypes errorType, object values = null, Exception innerException = null, [CallerMemberName] string methodName = null)
            : base($"{layer.GetDescription()}{methodName}", errorType, values, innerException)
        {
        }
    }
}
