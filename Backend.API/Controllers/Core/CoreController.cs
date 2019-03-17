using System;
using System.Runtime.CompilerServices;
using Backend.Manager.Helpers;
using Backend.Manager.Helpers.Errors;
using Backend.Manager.Helpers.Errors.CustomErrors;
using Backend.Manager.Helpers.Extension;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Backend.API.Controllers.Core
{
    public class CoreController : ControllerBase
    {
        public CoreController(IStringLocalizer localizer, IStringLocalizer<SharedResources> sharedLocalizer)
        {
            this.Localizer = localizer;
            this.SharedLocalizer = sharedLocalizer;
        }

        public CoreController(IStringLocalizer<SharedResources> sharedLocalizer)
        {
            this.SharedLocalizer = sharedLocalizer;
        }

        public IStringLocalizer Localizer { get; protected set; }

        public IStringLocalizer<SharedResources> SharedLocalizer { get; protected set; }

        #region " [LOGGER FUNCTIONS] "

        protected IActionResult LogAndReturnCustomError(BaseCustomError ex, ILogger logger, [CallerMemberName]string methodName = null)
        {
            var message = string.Format(this.SharedLocalizer.GetString("GlobalUnexpectedError").Value, Guid.NewGuid().ToString());
            methodName = string.IsNullOrWhiteSpace(ex.ErrorResponse.DeveloperMessage.Method) ? methodName : ex.ErrorResponse.DeveloperMessage.Method;

            ex.ErrorResponse
                .SetError()
                .SetStatusCode(StatusCodes.Status500InternalServerError)
                .SetUserMessage(message)
                .SetMethodName(methodName);

            logger.LogError(ex, ex.ErrorResponse.UserMessage);
            return this.StatusCode(StatusCodes.Status500InternalServerError, ex.ErrorResponse);
        }

        protected IActionResult LogAndReturnCustomError(Exception ex, ILogger logger, [CallerMemberName]string methodName = null)
        {
            logger.LogError(ex, this.SharedLocalizer.GetString("GlobalUnexpectedError").Value);
            return this.SetErrorResponse(ex, this.SharedLocalizer.GetString(ErrorTypes.UNEXPECTED_ERROR.ToString()).Value, $"{BackendLayersNames.API}.{methodName}");
        }

        protected IActionResult LogAndReturnCustomError(ILogger logger, int code, ErrorTypes errorType, object objectID = null, [CallerMemberName]string methodName = null)
        {
            var ex = new ApplicationApiException(BackendLayersNames.API, errorType, new { Guid = Guid.NewGuid().ToString(), ID = objectID }, methodName: methodName);
            logger.LogError($"{ex.ErrorResponse.DeveloperMessage}");

            ex.ErrorResponse
                .SetError()
                .SetStatusCode(code)
                .SetUserMessage(this.SharedLocalizer.GetString(errorType.ToString()).Value);

            return this.StatusCode(code, ex.ErrorResponse);
        }

        /// <summary>
        /// Used to set the response error based on the Global Exception.
        /// </summary>
        /// <param name="ex">{Exception} The Error.</param>
        /// <param name="message">{String} The user friendly message.</param>
        /// <param name="methodName">{String:Optional} The name of the method that generated the error.</param>
        /// <returns>{IActionResult} The Response Error.</returns>
        private IActionResult SetErrorResponse(Exception ex, string message = "", [CallerMemberName]string methodName = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                message = ex.Message;
            }

            var error = new ErrorResponse()
                .SetError()
                .SetStatusCode(StatusCodes.Status500InternalServerError)
                .SetUserMessage(message)
                .SetMethodName(methodName)
                .SetMessage(ex.Message)
                .SetInnerException(ex.InnerException?.Message)
                .SetStackTrace(ex.StackTrace);

            return this.StatusCode(StatusCodes.Status500InternalServerError, error);
        }

        #endregion
    }
}
