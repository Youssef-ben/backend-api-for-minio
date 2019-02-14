using System;
using System.Runtime.CompilerServices;
using Backend.Manager.Helpers.Extension;

namespace Backend.Manager.Helpers.Errors.CustomErrors
{
    public class ApplicationTestException : BaseCustomError
    {
        public ApplicationTestException()
        {
        }

        public ApplicationTestException(string message)
            : base(message)
        {
        }

        public ApplicationTestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ApplicationTestException(BackendLayersNames layer, ErrorTypes errorType, object values = null, Exception innerException = null, [CallerMemberName] string methodName = null)
            : base($"{layer.GetDescription()}{methodName}", errorType, values, innerException)
        {
        }
    }
}
