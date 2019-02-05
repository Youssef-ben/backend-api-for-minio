using System.Linq;
using Backend.Fileupload.API.Controllers.Core;
using Backend.Fileupload.API.Helpers.Errors;
using Backend.Fileupload.API.Helpers.Errors.Extension;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;

namespace Backend.Fileupload.API.Config
{
    public class ValidateModelAttribute : ResultFilterAttribute
    {
        private readonly IStringLocalizer<SharedResources> localizer;

        public ValidateModelAttribute(IStringLocalizer<SharedResources> localizer)
        {
            this.localizer = localizer;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.ModelState.IsValid)
            {
                return;
            }

            var validationErros = context.ModelState.Keys
                .SelectMany(key => context.ModelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                .ToList();

            var error = new ErrorResponse()
                .SetError()
                .SetStatusCode(StatusCodes.Status500InternalServerError)
                .SetUserMessage(this.localizer.GetString("EMPTY_VALUES").Value)
                .SetValdidationErrors(validationErros)
                .SetMethodName("API.ValidateModelAttribute")
                .SetMessage("Error Occurred while trying to validate the model state. please refer to the {validation_errors} field.");

            context.Result = new BadRequestObjectResult(error);
        }
    }
}
