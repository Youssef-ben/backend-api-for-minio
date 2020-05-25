namespace Backend.Minio.Api.Controllers.V1
{
    using System.Collections.Generic;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using Backend.Minio.Api.Controllers.Core;
    using Backend.Minio.Manager.Helpers;
    using Backend.Minio.Manager.Helpers.Api.Response.Models;
    using Backend.Minio.Manager.Implementation.Buckets.Items;
    using Backend.Minio.Manager.Models;
    using global::Minio.DataModel;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [ApiController]
    [ApiVersion("1.0")]
    [ControllerName("Buckets - items")]
    [Route("v{version:apiVersion}/buckets/{bucket}/items")]
    [Produces(MediaTypeNames.Application.Json)]
    public class BucketsItemsController : CustomBaseController
    {
        private readonly IBucketItemsManager manager;

        public BucketsItemsController(ILogger<BucketsItemsController> logger, IBucketItemsManager manager)
            : base(logger)
        {
            this.manager = manager;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Item>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadItemAsync(string bucket, IFormFile file)
        {
            if (string.IsNullOrWhiteSpace(bucket) || file is null)
            {
                return this.LogBadRequest(Constants.API_FIELDS_INVALID_VALIDATION, Constants.API_FIELDS_INVALID_VALIDATION_ID);
            }

            var result = await this.manager.UploadItemAsync(new BucketItem
            {
                Bucket = bucket,
                ItemKey = file.FileName,
                FormFile = file,
            });

            return this.LogSuccess(result);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<Item>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateItemAsync(string bucket, IFormFile file)
        {
            if (string.IsNullOrWhiteSpace(bucket) || file is null)
            {
                return this.LogBadRequest(Constants.API_FIELDS_INVALID_VALIDATION, Constants.API_FIELDS_INVALID_VALIDATION_ID);
            }

            var result = await this.manager.UpdateItemAsync(new BucketItem
            {
                Bucket = bucket,
                ItemKey = file.FileName,
                FormFile = file,
            });

            return this.LogSuccess(result);
        }

        [HttpDelete]
        [Route("{identifier}")]
        [ProducesResponseType(typeof(ApiResponse<Item>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateItemAsync(string bucket, string identifier)
        {
            if (string.IsNullOrWhiteSpace(bucket) || string.IsNullOrWhiteSpace(identifier))
            {
                return this.LogBadRequest(Constants.API_FIELDS_INVALID_VALIDATION, Constants.API_FIELDS_INVALID_VALIDATION_ID);
            }

            var result = await this.manager.RemoveItemAsync(new BucketItem
            {
                Bucket = bucket,
                ItemKey = identifier,
            });

            return this.LogSuccess(result);
        }

        [HttpGet]
        [Route("{identifier}")]
        [ProducesResponseType(typeof(ApiResponse<Item>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetItemExistsAsync(string bucket, string identifier)
        {
            var result = await this.manager.GetItemDetailsAsync(new BucketItem
            {
                Bucket = bucket,
                ItemKey = identifier,
            });

            return this.LogSuccess(result);
        }

        [HttpGet]
        [Route("{identifier}/download")]
        [ProducesResponseType(typeof(ApiResponse<Item>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadItemAsync(string bucket, string identifier)
        {
            if (string.IsNullOrWhiteSpace(bucket) || string.IsNullOrWhiteSpace(identifier))
            {
                return this.LogBadRequest(Constants.API_FIELDS_INVALID_VALIDATION, Constants.API_FIELDS_INVALID_VALIDATION_ID);
            }

            var result = await this.manager.DownloadItemAsync(new BucketItem
            {
                Bucket = bucket,
                ItemKey = identifier,
            });

            return this.File(result.StreamContent, result.Type, result.Name);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<ICollection<Item>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBucketItemsAsync(string bucket, int page = 1, int size = Constants.DEFAULT_PAGE_LIMITE)
        {
            if (string.IsNullOrWhiteSpace(bucket))
            {
                return this.LogBadRequest(Constants.API_FIELDS_INVALID_VALIDATION, Constants.API_FIELDS_INVALID_VALIDATION_ID);
            }

            var result = await this.manager.ListBucketItemsAsync(bucket, page, size);

            return this.LogSuccess(result);
        }
    }
}