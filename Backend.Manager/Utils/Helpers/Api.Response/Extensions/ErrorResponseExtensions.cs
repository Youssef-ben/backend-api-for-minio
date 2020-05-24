namespace Backend.Minio.Manager.Helpers.Api.Response.Extensions
{
    using Backend.Minio.Manager.Helpers.Api.Response.Models;

    /// <summary>
    /// Defines a Fluent methods to use with the Error Response Class.
    /// </summary>
    public static class ErrorResponseExtensions
    {
        public static ErrorResponse SetUserMessage(this ErrorResponse self, string message, string messageID)
        {
            self.Message = message;
            self.MessageID = messageID;
            return self;
        }

        public static ErrorResponse SetErrorResponseValues(this ErrorResponse self, object values)
        {
            self.DeveloperMessage.Values = values;
            return self;
        }

        public static ApiResponse<T> SetResponseData<T>(this ApiResponse<T> self, T values)
            where T : class
        {
            self.Result = values;
            return self;
        }

        public static ErrorResponse SetErrorType(this ErrorResponse self, string values)
        {
            self.ErrorType = values;
            return self;
        }

        public static ErrorResponse SetMessage(this ErrorResponse self, string message)
        {
            self.DeveloperMessage.Message = message;
            return self;
        }

        public static ErrorResponse SetMethodName(this ErrorResponse self, string name)
        {
            self.DeveloperMessage.Method = name;
            return self;
        }

        public static ErrorResponse SetInnerException(this ErrorResponse self, string message)
        {
            self.DeveloperMessage.InnerException = string.IsNullOrWhiteSpace(message) ? null : message;
            return self;
        }

        public static ErrorResponse SetStackTrace(this ErrorResponse self, string stackTrace)
        {
            self.DeveloperMessage.StackTrace = string.IsNullOrWhiteSpace(stackTrace) ? null : stackTrace;
            return self;
        }

        public static ErrorResponse SetErrorValues(this ErrorResponse self, object values)
        {
            self.DeveloperMessage.Values = values;
            return self;
        }
    }
}
