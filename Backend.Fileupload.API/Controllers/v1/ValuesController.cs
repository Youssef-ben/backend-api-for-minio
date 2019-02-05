using System;
using System.Collections.Generic;
using Backend.Fileupload.API.Controllers.Core;
using Backend.Fileupload.API.Helpers.Errors;
using Backend.Fileupload.API.Helpers.Errors.CustomErrors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Backend.Fileupload.API.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("api/v{version:apiVersion}/values")]
    public class ValuesController : CoreController
    {
        private readonly ILogger logger;

        public ValuesController(IStringLocalizer<SharedResources> sharedLocalizer, ILogger<ValuesController> logger)
            : base(sharedLocalizer)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Return the Values of the Controller
        /// </summary>
        /// <returns>ICollection of Object.</returns>
        ///  <response code="200">The list of objects.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        [ProducesResponseType(typeof(ICollection<MyValues>), 200)]
        public IActionResult Get()
        {
            try
            {
                this.logger.LogInformation($"{string.Format(BackendConstants.ApiStartedTheRequestProcess, Guid.NewGuid().ToString(), string.Empty)}");


                var list = new HashSet<MyValues>()
            {
                new MyValues()
                {
                    Guid = Guid.NewGuid(),
                    ValueName = "Name",
                    Description = "Description"
                },
                new MyValues()
                {
                    Guid = Guid.NewGuid(),
                    ValueName = "Name 2",
                    Description = "Description 2"
                },
                new MyValues()
                {
                    Guid = Guid.NewGuid(),
                    ValueName = "Name 3",
                    Description = "Description 3"
                },
            };

                return this.StatusCode(StatusCodes.Status200OK, list);
            }
            catch (BaseCustomError ex)
            {
                return this.LogAndReturnCustomError(ex, this.logger);
            }
            catch (Exception ex)
            {
                return this.LogAndReturnCustomError(ex, this.logger);
            }
        }

        internal class MyValues
        {
            public Guid Guid { get; set; }

            public string ValueName { get; set; }

            public string Description { get; set; }
        }
    }
}
