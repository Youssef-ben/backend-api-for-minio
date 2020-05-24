namespace Backend.Minio.Api.Controllers.Core
{
    using System.Runtime.CompilerServices;
    using Backend.Minio.Manager.Helpers;
    using Backend.Minio.Manager.Helpers.Api.Response.Extensions;
    using Backend.Minio.Manager.Helpers.Api.Response.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [ApiController]
    public class CustomBaseController : ControllerBase
    {
        private readonly ILogger logger;

        public CustomBaseController(ILogger logger)
        {
            this.logger = logger;
        }

        protected IActionResult LogSuccess<T>(T entity = null, int statusCode = StatusCodes.Status200OK, [CallerMemberName]string methodName = default)
            where T : class
        {
            this.logger.LogDebug(Constants.API_REPONSE_SUCCESS);

            var response = new ApiResponse<T>()
                .SetResponseData<T>(entity)
                .SetUserMessage(Constants.API_REPONSE_SUCCESS, Constants.API_REPONSE_SUCCESS_ID)
                .SetMessage(Constants.API_REPONSE_SUCCESS)
                .SetMethodName($"API.Method.{methodName}");

            return this.StatusCode(statusCode, response);
        }

        protected IActionResult LogNotFound(string message, string messageId = default, [CallerMemberName]string methodName = default)
        {
            this.logger.LogWarning($"API - {methodName} - {message}");

            messageId = string.IsNullOrWhiteSpace(messageId) ? Constants.API_NOT_FOUND_ID : messageId;

            var response = new ErrorResponse()
                .SetUserMessage(message, messageId)
                .SetMessage(Constants.API_REPONSE_FAILED)
                .SetMethodName($"API.Method.{methodName}");

            return this.StatusCode(StatusCodes.Status404NotFound, response);
        }

        protected IActionResult LogBadRequest(string message, string messageId = default, [CallerMemberName]string methodName = default)
        {
            this.logger.LogError($"API - {methodName} - {message}");

            messageId = string.IsNullOrWhiteSpace(messageId) ? Constants.API_BAD_REQUEST_ERROR_MESSAGE_ID : messageId;

            var response = new ErrorResponse()
                .SetUserMessage(message, messageId)
                .SetMessage(Constants.API_REPONSE_FAILED)
                .SetMethodName($"API.Method.{methodName}");

            return this.StatusCode(StatusCodes.Status400BadRequest, response);
        }
    }
}