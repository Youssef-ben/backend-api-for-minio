using Minio.DataModel;

namespace Backend.Minio.Api.Controllers.v1
{
    using System.Collections.Generic;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using Backend.Minio.Api.Controllers.Core;
    using Backend.Minio.Api.Models;
    using Backend.Minio.Manager.Helpers;
    using Backend.Minio.Manager.Helpers.Api.Response.Models;
    using Backend.Minio.Manager.Helpers.Extension;
    using Backend.Minio.Manager.Implementation.Buckets;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [ApiController]
    [ApiVersion("1.0")]
    [ControllerName("Buckets")]
    [Route("v{version:apiVersion}/buckets")]
    [Produces(MediaTypeNames.Application.Json)]
    public class BucketController : CustomBaseController
    {
        private readonly IBucketManager manager;

        public BucketController(ILogger<BucketController> logger, IBucketManager manager)
            : base(logger)
        {
            this.manager = manager;
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ApiResponse<Bucket>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBucketAsync([FromBody] BucketDto bucket)
        {
            if (bucket is null || string.IsNullOrWhiteSpace(bucket.Name))
            {
                return this.LogBadRequest(Constants.API_FIELDS_INVALID_VALIDATION, Constants.API_FIELDS_INVALID_VALIDATION_ID);
            }

            var result = await this.manager.CreateBucketAsync(bucket.Name);

            return this.LogSuccess(result, StatusCodes.Status201Created);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<Bucket>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RenameBucketAsync([FromBody] BucketDto bucket)
        {
            if (bucket is null || string.IsNullOrWhiteSpace(bucket.Name) || string.IsNullOrWhiteSpace(bucket.NewName))
            {
                return this.LogBadRequest(Constants.API_FIELDS_INVALID_VALIDATION, Constants.API_FIELDS_INVALID_VALIDATION_ID);
            }

            if (!await this.manager.BucketExistsAsync(bucket.Name))
            {
                return this.LogNotFound(Constants.API_NOT_FOUND.FormatText(bucket.Name));
            }

            var result = await this.manager.RenameBucketAsync(bucket.Name, bucket.NewName);

            return this.LogSuccess(result);
        }

        [HttpDelete]
        [Route("{identifier}")]
        [ProducesResponseType(typeof(ApiResponse<Bucket>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBucketAsync(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                return this.LogBadRequest(Constants.API_FIELDS_INVALID_VALIDATION, Constants.API_FIELDS_INVALID_VALIDATION_ID);
            }

            if (!await this.manager.BucketExistsAsync(identifier))
            {
                return this.LogNotFound(Constants.API_NOT_FOUND.FormatText(identifier));
            }

            var result = await this.manager.DeleteBucketAsync(identifier);

            return this.LogSuccess(result);
        }

        [HttpGet]
        [Route("{identifier}/exists")]
        [ProducesResponseType(typeof(ApiResponse<Bucket>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DoesBucketExistsAsync(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                return this.LogBadRequest(Constants.API_FIELDS_INVALID_VALIDATION, Constants.API_FIELDS_INVALID_VALIDATION_ID);
            }

            if (!await this.manager.BucketExistsAsync(identifier))
            {
                return this.LogNotFound(Constants.API_NOT_FOUND.FormatText(identifier));
            }

            var result = await this.manager.GetBucketAsync(identifier);
            return this.LogSuccess(result);
        }

        [HttpGet]
        [Route("{identifier}")]
        [ProducesResponseType(typeof(ApiResponse<Bucket>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBucketAsync(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                return this.LogBadRequest(Constants.API_FIELDS_INVALID_VALIDATION, Constants.API_FIELDS_INVALID_VALIDATION_ID);
            }

            if (!await this.manager.BucketExistsAsync(identifier))
            {
                return this.LogNotFound(Constants.API_NOT_FOUND.FormatText(identifier));
            }

            var result = await this.manager.GetBucketAsync(identifier);
            return this.LogSuccess(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<ICollection<Bucket>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync(int pageId = 1, int pageSize = IBucketManager.DEFAULT_PAGE_LIMITE)
        {
            var result = await this.manager.GetAllBucketsAsync(pageId, pageSize);

            return this.LogSuccess(result);
        }
    }
}