using System.Collections.Generic;

namespace Backend.Fileupload.API.Helpers.Errors.Extension
{
    /// <summary>
    /// Extension used to define a Fluent method to use for the Error Response Class.
    /// </summary>
    public static class ErrorResponseExtension
    {
        public static ErrorResponse SetError(this ErrorResponse self, bool isError = true)
        {
            self.Error = isError ? 1 : 0;
            return self;
        }

        public static ErrorResponse SetStatusCode(this ErrorResponse self, int statusCode)
        {
            // Set the default value of the Code to Internal Error.
            self.Code = statusCode <= 0 ? 500 : statusCode;
            return self;
        }

        public static ErrorResponse SetUserMessage(this ErrorResponse self, string message)
        {
            self.UserMessage = message;
            return self;
        }

        public static ErrorResponse SetValdidationErrors(this ErrorResponse self, ICollection<ValidationError> values)
        {
            self.ValidationErrors = values.Count == 0 ? null : values;
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

        public static ErrorResponse SetStackTrace(this ErrorResponse self, string name)
        {
            self.DeveloperMessage.StackTrace = string.IsNullOrWhiteSpace(name) ? null : name;
            return self;
        }

        public static ErrorResponse SetErrorValues(this ErrorResponse self, object values)
        {
            self.DeveloperMessage.Values = values;
            return self;
        }

        public static ErrorResponse SetDeveloperObject(this ErrorResponse self, DevelopperMessage developperObject)
        {
            self.DeveloperMessage = developperObject;
            return self;
        }

        public static DevelopperMessage GetDeveloperObject(this ErrorResponse self)
        {
            return self.DeveloperMessage;
        }
    }
}
