namespace Backend.Minio.Api.Configurations
{
    using System.Linq;
    using Backend.Minio.Manager.Helpers;
    using Backend.Minio.Manager.Helpers.Api.Response.Extensions;
    using Backend.Minio.Manager.Helpers.Api.Response.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class ValidateModelAttribute : ResultFilterAttribute
    {
        public ValidateModelAttribute()
        {
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.ModelState.IsValid)
            {
                return;
            }

            var validationErrors = context.ModelState.Keys
                .SelectMany(key => context.ModelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                .ToList();

            var error = new ErrorResponse()
                .SetMethodName("API.ValidateModelAttribute(...)")
                .SetErrorResponseValues(validationErrors)
                .SetUserMessage(Constants.API_INVALID_FIELDS_VALIDATION, Constants.API_INVALID_FIELDS_VALIDATION_ID)
                .SetMessage(Constants.API_INVALID_FIELDS_VALIDATION_ERROR);

            context.Result = new BadRequestObjectResult(error);
        }
    }
}
