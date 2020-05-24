namespace Backend.Minio.Manager.Helpers.Api.Response.Custom
{
    using System;
    using System.Runtime.CompilerServices;

    public class ApplicationManagerException : BaseCustomError
    {
        public ApplicationManagerException()
        {
        }

        public ApplicationManagerException(string message, string messageId, object values = null, Exception innerException = null, [CallerMemberName] string methodName = null)
            : base($"Manager.{methodName}", message, messageId, values, innerException)
        {
        }
    }
}
