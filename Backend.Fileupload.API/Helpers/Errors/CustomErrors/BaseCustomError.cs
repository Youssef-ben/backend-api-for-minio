using System;
using Backend.Fileupload.API.Helpers.Errors.Extension;

namespace Backend.Fileupload.API.Helpers.Errors.CustomErrors
{
    public class BaseCustomError : Exception
    {
        public BaseCustomError()
        {
        }

        public BaseCustomError(string message)
            : base(message)
        {
        }

        public BaseCustomError(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public BaseCustomError(string methodName, ErrorTypes errorType, object values = null, Exception innerException = null)
            : base(errorType.GetDescription(), innerException)
        {
            this.ErrorResponse = new ErrorResponse()
                .SetError()
                .SetMethodName(methodName)
                .SetMessage(errorType.GetDescription())
                .SetErrorValues(values)
                .SetInnerException(innerException?.Message)
                .SetStackTrace(innerException?.StackTrace);
        }

        public ErrorResponse ErrorResponse { get; set; }
    }
}
