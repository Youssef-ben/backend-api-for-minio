namespace Backend.Minio.Manager.Helpers.Api.Response.Custom
{
    using System;
    using Backend.Minio.Manager.Helpers.Api.Response.Extensions;
    using Backend.Minio.Manager.Helpers.Api.Response.Models;

    public abstract class BaseCustomError : Exception
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

        public BaseCustomError(string methodName, string message, string messageId, object values = null, Exception innerException = null)
            : base(message, innerException)
        {
            this.ErrorResponse = new ErrorResponse()
                .SetMethodName(methodName)
                .SetUserMessage(message, messageId)
                .SetMessage(message)
                .SetErrorValues(values)
                .SetInnerException(innerException?.Message)
                .SetStackTrace(innerException?.StackTrace);
        }

        public ErrorResponse ErrorResponse { get; set; }
    }
}
