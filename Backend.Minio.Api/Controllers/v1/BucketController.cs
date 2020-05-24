using Minio.DataModel;

namespace Backend.Minio.Api.Controllers.v1
{
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
        [ProducesResponseType(typeof(ApiResponse<Bucket>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBucketAsync([FromBody] BucketDto bucket)
        {
            var result = await this.manager.CreateBucketAsync(bucket.Name);

            return this.LogSuccess(result, StatusCodes.Status201Created);
        }

        [HttpGet]
        [Route("{identifier}/exists")]
        [ProducesResponseType(typeof(ApiResponse<Bucket>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DoesBucketExistsAsync(string identifier)
        {
            var exists = await this.manager.BucketExistsAsync(identifier);
            if (!exists)
            {
                return this.LogNotFound(Constants.API_NOT_FOUND.FormatText(identifier));
            }

            var result = await this.manager.GetBucketAsync(identifier);
            return this.LogSuccess(result);
        }
    }
}