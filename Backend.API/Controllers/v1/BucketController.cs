using System;
using System.Threading.Tasks;
using Backend.API.Controllers.Core;
using Backend.API.Models;
using Backend.Manager.Helpers.Errors;
using Backend.Manager.Helpers.Errors.CustomErrors;
using Backend.Manager.Implementation.Buckets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Backend.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("api/v{version:apiVersion}/bucket")]
    public class BucketController : CoreController
    {
        private readonly ILogger logger;
        private readonly IBucketManager manager;

        public BucketController(
            IStringLocalizer<SharedResources> sharedLocalizer,
            ILogger<BucketController> logger,
            IBucketManager manager) : base(sharedLocalizer)
        {
            this.logger = logger;
            this.manager = manager;
        }

        /// <summary>
        /// Check if a bucket exists or not.
        /// </summary>
        /// <param name="id">Bucket.</param>
        /// <returns>Success with value to indicate that the bucket exists or not.</returns>
        ///  <response code="200">Success Status.</response>
        ///  <response code="400">The specified name is empty.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [Route("{id}/exists")]
        [ProducesResponseType(typeof(SuccessResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> BucketExistsAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return this.LogAndReturnCustomError(this.logger, StatusCodes.Status400BadRequest, ErrorTypes.CREATE);
            }

            try
            {
                var result = await this.manager.SetBucket(id).BucketExistsAsync();

                var returnObject = new SuccessResponse() { Error = 0, UserMessage = $"Bucket [{id}] Exists!" };
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
        /// Check if a bucket exists or not.
        /// </summary>
        /// <param name="id">Bucket.</param>
        /// <returns>Success with value to indicate that the bucket exists or not.</returns>
        ///  <response code="200">Success Status.</response>
        ///  <response code="400">The specified name is empty.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(SuccessResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> GetBucketInfoAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return this.LogAndReturnCustomError(this.logger, StatusCodes.Status400BadRequest, ErrorTypes.CREATE);
            }

            try
            {
                var result = await this.manager.SetBucket(id).GetBucketAsync();

                var returnObject = new SuccessResponse() { Error = 0, UserMessage = $"Found Bucket {id}", Bucket = result };
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
        /// Create a bucket based on the specified name if not exits.
        /// </summary>
        /// <param name="bucket">Bucket.</param>
        /// <returns>Success</returns>
        ///  <response code="200">Success Status.</response>
        ///  <response code="400">The specified name is empty.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [ProducesResponseType(typeof(SuccessResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> CreateBucketAsync([FromBody] BucketDto bucket)
        {
            if (bucket is null || string.IsNullOrWhiteSpace(bucket.Name))
            {
                return this.LogAndReturnCustomError(this.logger, StatusCodes.Status400BadRequest, ErrorTypes.CREATE);
            }

            try
            {
                await this.manager.SetBucket(bucket.Name).CreateBucketAsync();

                var result = await this.manager.GetBucketAsync();
                var returnObject = new SuccessResponse() { Error = 0, UserMessage = "Bucket Successfully Created!", Bucket = result };
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
        /// Rename a bucket if exits.
        /// </summary>
        /// <param name="bucket">Bucket.</param>
        /// <returns>Success</returns>
        ///  <response code="200">Success Status.</response>
        ///  <response code="400">The specified name is empty.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut]
        [ProducesResponseType(typeof(SuccessResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> RenameBucketAsync([FromBody] BucketDto bucket)
        {
            if (bucket is null || string.IsNullOrWhiteSpace(bucket.Name) || string.IsNullOrWhiteSpace(bucket.NewName))
            {
                return this.LogAndReturnCustomError(this.logger, StatusCodes.Status400BadRequest, ErrorTypes.CREATE);
            }

            try
            {
                var result = await this.manager.SetBucket(bucket.Name).RenameBucketAsync(bucket.NewName);
                if (!result)
                {
                    return this.LogAndReturnCustomError(this.logger, StatusCodes.Status400BadRequest, ErrorTypes.UPDATE, bucket);
                }


                var newBucket = await this.manager.SetBucket(bucket.NewName).GetBucketAsync();
                var returnObject = new SuccessResponse() { Error = 0, UserMessage = "Bucket Successfully Updated!", Bucket = newBucket };
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
        /// Delete a bucket if exits.
        /// </summary>
        /// <param name="bucket">Bucket.</param>
        /// <returns>Success</returns>
        ///  <response code="200">Success Status.</response>
        ///  <response code="400">The specified name is empty.</response>
        /// <response code="500">Internal server error.</response>
        [HttpDelete]
        [ProducesResponseType(typeof(SuccessResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> DeleteBucketAsync([FromBody] BucketDto bucket)
        {
            if (bucket is null || string.IsNullOrWhiteSpace(bucket.Name))
            {
                return this.LogAndReturnCustomError(this.logger, StatusCodes.Status400BadRequest, ErrorTypes.DELETE);
            }

            try
            {
                var result = await this.manager.SetBucket(bucket.Name).DeleteBucketAsync(bucket.NewName);
                if (!result)
                {
                    return this.LogAndReturnCustomError(this.logger, StatusCodes.Status400BadRequest, ErrorTypes.DELETE, bucket);
                }

                var returnObject = new SuccessResponse() { Error = 0, UserMessage = "Bucket Successfully Deleted!", Bucket = null };
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