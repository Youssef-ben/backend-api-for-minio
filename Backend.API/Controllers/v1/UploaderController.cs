using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Backend.API.Controllers.Core;
using Backend.Manager.Helpers.Errors;
using Backend.Manager.Helpers.Errors.CustomErrors;
using Backend.Manager.Implementation.Uploader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Minio.DataModel;

namespace Backend.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("api/v{version:apiVersion}/bucket/{id}/uploader")]
    public class UploaderController : CoreController
    {
        private readonly ILogger logger;
        private readonly IUploaderManager manager;

        public UploaderController(
            IStringLocalizer<SharedResources> sharedLocalizer,
            ILogger<BucketController> logger,
            IUploaderManager manager) : base(sharedLocalizer)
        {
            this.logger = logger;
            this.manager = manager;
        }

        /// <summary>
        /// Check if the specified file exists or not.
        /// </summary>
        /// <param name="id">Bucket name.</param>
        /// <param name="name">File name.</param>
        /// <returns>Success with values.</returns>
        ///  <response code="200">Success Status.</response>
        ///  <response code="400">The specified name is empty.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [Route("{name}/exists")]
        [ProducesResponseType(typeof(SuccessResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> FileExistsAsync(string id, string name)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name))
            {
                return this.LogAndReturnCustomError(this.logger, StatusCodes.Status400BadRequest, ErrorTypes.FETCH);
            }

            try
            {
                if (!await this.manager.SetBucket(id).FileExistsAsync(name))
                {
                    return this.LogAndReturnCustomError(this.logger, StatusCodes.Status404NotFound, ErrorTypes.NOT_FOUND, name);
                }

                var returnObject = new SuccessResponse() { Error = 0, UserMessage = $"File [{name}] Exists!" };
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
        /// Get all the Bucket Files.
        /// </summary>
        /// <param name="id">Bucket name.</param>
        /// <param name="page">Bucket page.</param>
        /// <param name="size">Page size.</param>
        /// <returns>Success with values.</returns>
        ///  <response code="200">Success Status.</response>
        ///  <response code="400">The specified name is empty.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [ProducesResponseType(typeof(ICollection<Item>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> GetFilesAsync(string id, int page = 1, int size = 20)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return this.LogAndReturnCustomError(this.logger, StatusCodes.Status400BadRequest, ErrorTypes.FETCH);
            }

            try
            {
                var result = await this.manager.SetBucket(id).ListBucketFilesAsync(page, size);

                var returnObject = new { Error = 0, Items = result };
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
        /// Get The Specified file in the current bucket.
        /// </summary>
        /// <param name="id">Bucket name.</param>
        /// <param name="name">File name.</param>
        /// <returns>Success with values.</returns>
        ///  <response code="200">Success Status.</response>
        ///  <response code="400">The specified name is empty.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [Route("{name}")]
        [ProducesResponseType(typeof(Item), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> GetFileAsync(string id, string name)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name))
            {
                return this.LogAndReturnCustomError(this.logger, StatusCodes.Status400BadRequest, ErrorTypes.FETCH);
            }

            try
            {
                var result = await this.manager.SetBucket(id).GetFileAsync(name);

                if (result is null)
                {
                    return this.LogAndReturnCustomError(this.logger, StatusCodes.Status404NotFound, ErrorTypes.NOT_FOUND, name);
                }

                var returnObject = new { Error = 0, Item = result };
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
        /// Upload the file to the current Bucket.
        /// </summary>
        /// <param name="id">Bucket name.</param>
        /// <param name="file">File to upload.</param>
        /// <returns>Success with values.</returns>
        ///  <response code="200">Success Status.</response>
        ///  <response code="400">The specified name is empty.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Item), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> UploadFileAsync(string id, IFormFile file)
        {
            if (string.IsNullOrWhiteSpace(id) || file is null)
            {
                return this.LogAndReturnCustomError(this.logger, StatusCodes.Status400BadRequest, ErrorTypes.CREATE);
            }

            try
            {
                var result = await this.manager.SetBucket(id).UploadFileAsync(file);

                var returnObject = new { Error = 0, Item = result };
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
        /// Update an existing file in the current Bucket.
        /// </summary>
        /// <param name="id">Bucket name.</param>
        /// <param name="file">File to update.</param>
        /// <returns>Success with values.</returns>
        ///  <response code="200">Success Status.</response>
        ///  <response code="400">The specified name is empty.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut]
        [ProducesResponseType(typeof(Item), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> UpdateFileAsync(string id, IFormFile file)
        {
            if (string.IsNullOrWhiteSpace(id) || file is null)
            {
                return this.LogAndReturnCustomError(this.logger, StatusCodes.Status400BadRequest, ErrorTypes.CREATE);
            }

            try
            {
                var result = await this.manager.SetBucket(id).UpdateFileAsync(file);

                var returnObject = new { Error = 0, Item = result };
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
        /// Download the specified file from the current Bucket.
        /// </summary>
        /// <param name="id">Bucket name.</param>
        /// <param name="name">File to download.</param>
        /// <returns>Success with values.</returns>
        ///  <response code="200">Success Status.</response>
        ///  <response code="400">The specified name is empty.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [Route("{name}/download")]
        [ProducesResponseType(typeof(File), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> DownloadFileAsync(string id, string name)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name))
            {
                return this.LogAndReturnCustomError(this.logger, StatusCodes.Status400BadRequest, ErrorTypes.FETCH);
            }

            try
            {
                var result = await this.manager.SetBucket(id).DownloadFileAsync(name);

                return File(result.Content, result.Type, result.Name);
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
        /// Remove the specified file from the current Bucket.
        /// </summary>
        /// <param name="id">Bucket name.</param>
        /// <param name="name">File name.</param>
        /// <returns>Success with values.</returns>
        ///  <response code="200">Success Status.</response>
        ///  <response code="400">The specified name is empty.</response>
        /// <response code="500">Internal server error.</response>
        [HttpDelete]
        [Route("{name}")]
        [ProducesResponseType(typeof(SuccessResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> RemoveFileAsync(string id, string name)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name))
            {
                return this.LogAndReturnCustomError(this.logger, StatusCodes.Status400BadRequest, ErrorTypes.FETCH);
            }

            try
            {
                var result = await this.manager.SetBucket(id).RemoveFileAsync(name);

                if (!result)
                {
                    return this.LogAndReturnCustomError(this.logger, StatusCodes.Status400BadRequest, ErrorTypes.ERROR_WHILE_DELETING_THE_DOCUMENT, name);
                }

                var returnObject = new SuccessResponse { Error = 0, UserMessage = $"The file [{name}] was deleted successfully form the bucket [{id}]" };
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