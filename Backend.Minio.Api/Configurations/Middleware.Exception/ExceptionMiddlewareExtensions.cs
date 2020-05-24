namespace Facilis.Api.Helpers.Exception
{
    using System;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using Backend.Minio.Api.Configurations.Middleware.Exception;
    using Backend.Minio.Manager.Helpers;
    using Backend.Minio.Manager.Helpers.Api.Response.Custom;
    using Backend.Minio.Manager.Helpers.Api.Response.Extensions;
    using Backend.Minio.Manager.Helpers.Api.Response.Models;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder ConfigureExceptionHandler(this IApplicationBuilder self)
        {
            return self.UseMiddleware<ExceptionMiddleware>();
        }

        public static Task LogInternalServerError(this Exception self, HttpContext context, ILogger logger)
        {
            var error = new ErrorResponse()
                .SetMethodName("Api")
                .SetUserMessage(Constants.API_UNEXPECTED_ERROR_MESSAGE, Constants.API_UNEXPECTED_ERROR_MESSAGE_ID)
                .SetMessage(self.Message)
                .SetInnerException(self.InnerException?.Message)
                .SetStackTrace(self.StackTrace);

            var jsonResponse = JsonConvert.SerializeObject(error);

            logger.LogError(self, Constants.API_UNEXPECTED_ERROR_MESSAGE);

            // Set Response
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return context.Response.WriteAsync(jsonResponse);
        }

        public static Task LogBadRequest(this BaseCustomError self, HttpContext context, ILogger logger)
        {
            // Set Error
            if (string.IsNullOrWhiteSpace(self.ErrorResponse.Message))
            {
                self.ErrorResponse.Message = self.ErrorResponse.DeveloperMessage.Message;
            }

            var jsonResponse = JsonConvert.SerializeObject(self.ErrorResponse);

            logger.LogError(self, self.ErrorResponse.Message);

            // Set Response
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
