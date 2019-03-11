using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.API.Controllers.Core;
using Backend.Manager.Helpers.Errors;
using Backend.Manager.Helpers.Errors.CustomErrors;
using Backend.Manager.Implementation.Searcher;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Minio.DataModel;

namespace Backend.API.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("api/v{version:apiVersion}/bucket/{id}")]
    public class SearcherController : CoreController
    {
        private readonly ILogger logger;
        private readonly ISearchManager manager;

        public SearcherController(
            IStringLocalizer<SharedResources> sharedLocalizer,
            ILogger<SearcherController> logger,
            ISearchManager manager) : base(sharedLocalizer)
        {
            this.logger = logger;
            this.manager = manager;
        }


        /// <summary>
        /// Autocomplete bucket names.
        /// </summary>
        /// <param name="id">Bucket.</param>
        /// <param name="term">Search values.</param>
        /// <param name="page">Page index.</param>
        /// <param name="size">Data rows in a page.</param>
        /// <returns>Success with value to indicate that the bucket exists or not.</returns>
        ///  <response code="200">Success Status.</response>
        ///  <response code="400">The specified name is empty.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [Route("suggest")]
        [ProducesResponseType(typeof(ICollection<string>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> SuggestBucketNamesAsync(string id, string term, int page = 0, int size = 20)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return this.LogAndReturnCustomError(this.logger, StatusCodes.Status400BadRequest, ErrorTypes.FETCH);
            }

            try
            {
                var result = await this.manager.SetBucket(id).AutoCompleteBucketsByNameAsync(term, page, size);

                var returnObject = new { Error = 0, results = result };
                return this.StatusCode(StatusCodes.Status200OK, returnObject);
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

        /// <summary>
        /// Autocomplete bucket names.
        /// </summary>
        /// <param name="id">Bucket.</param>
        /// <param name="term">Search values.</param>
        /// <param name="page">Page index.</param>
        /// <param name="size">Data rows in a page.</param>
        /// <returns>Success with value to indicate that the bucket exists or not.</returns>
        ///  <response code="200">Success Status.</response>
        ///  <response code="400">The specified name is empty.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [Route("suggest/files")]
        [ProducesResponseType(typeof(ICollection<string>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> SuggestFileNamesAsync(string id, string term, int page = 0, int size = 20)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return this.LogAndReturnCustomError(this.logger, StatusCodes.Status400BadRequest, ErrorTypes.FETCH);
            }

            try
            {
                var result = await this.manager.SetBucket(id).AutoCompleteFilesByNameAsync(term, page, size);

                var returnObject = new { Error = 0, results = result };
                return this.StatusCode(StatusCodes.Status200OK, returnObject);
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

        /// <summary>
        /// Search for Files by name.
        /// </summary>
        /// <param name="id">Bucket.</param>
        /// <param name="term">Search values.</param>
        /// <param name="page">Page index.</param>
        /// <param name="size">Data rows in a page.</param>
        /// <returns>Success with value to indicate that the bucket exists or not.</returns>
        ///  <response code="200">Success Status.</response>
        ///  <response code="400">The specified name is empty.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [Route("search/names")]
        [ProducesResponseType(typeof(ICollection<Item>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> SearchFilesByNameAsync(string id, string term, int page = 0, int size = 20)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return this.LogAndReturnCustomError(this.logger, StatusCodes.Status400BadRequest, ErrorTypes.FETCH);
            }

            try
            {
                var result = await this.manager.SetBucket(id).SearchByNameAsync(term, page, size);

                var returnObject = new { Error = 0, results = result };
                return this.StatusCode(StatusCodes.Status200OK, returnObject);
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

        /// <summary>
        /// Search for Files by Content.
        /// </summary>
        /// <param name="id">Bucket.</param>
        /// <param name="term">Search values.</param>
        /// <param name="page">Page index.</param>
        /// <param name="size">Data rows in a page.</param>
        /// <returns>Success with value to indicate that the bucket exists or not.</returns>
        ///  <response code="200">Success Status.</response>
        ///  <response code="400">The specified name is empty.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [Route("search/content")]
        [ProducesResponseType(typeof(ICollection<Item>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> SearchFilesByContentAsync(string id, string term, int page = 0, int size = 20)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return this.LogAndReturnCustomError(this.logger, StatusCodes.Status400BadRequest, ErrorTypes.FETCH);
            }

            try
            {
                var result = await this.manager.SetBucket(id).SearchByContentAsync(term, page, size);

                var returnObject = new { Error = 0, results = result };
                return this.StatusCode(StatusCodes.Status200OK, returnObject);
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
    }
}