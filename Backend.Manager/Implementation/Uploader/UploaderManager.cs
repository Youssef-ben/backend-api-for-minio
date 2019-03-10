namespace Backend.Manager.Implementation.Uploader
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Backend.Manager.Helpers.Errors;
    using Backend.Manager.Helpers.Extension;
    using Backend.Manager.Repository;
    using Backend.Manager.Utils.Models;
    using Backend.Manager.Utils.Models.ConfigModels;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Minio;
    using Minio.DataModel;
    using Minio.Exceptions;

    public class UploaderManager : IUploaderManager
    {
        private readonly MinioClient minioClient;
        private readonly BackendConfiguration configuration;

        private readonly ILogger logger;
        private IElasticsearchRepository eslasticRepository;

        private string bucket = string.Empty;

        public UploaderManager(ILogger<UploaderManager> logger, IOptions<AppsettingsModel> config, IElasticsearchRepository elasticsearchRepository, MinioClient minioClient)
        {
            this.logger = logger;
            this.configuration = config.Value.Minio;

            this.eslasticRepository = elasticsearchRepository;
            this.minioClient = minioClient;
        }

        public UploaderManager SetBucket(string name)
        {
            name = name.SanitizeString();

            this.bucket = string.IsNullOrWhiteSpace(name) ? this.configuration.DefaultIndex.ToLower() : name.ToLower();

            this.eslasticRepository = this.eslasticRepository.SetBucketIndex(this.bucket);

            return this;
        }

        public async Task<bool> FileExistsAsync(string filename)
        {
            await this.CurrentBucketExistsAsync();

            try
            {
                var result = await this.minioClient.StatObjectAsync(this.bucket, filename);
            }
            catch (ObjectNotFoundException)
            {
                return false;
            }

            return true;
        }

        public async Task<ICollection<Item>> ListBucketFilesAsync()
        {
            await this.CurrentBucketExistsAsync();

            return await this.minioClient.GetBucketItemsAsync(this.bucket);
        }

        public async Task<bool> RemoveFileAsync(string filename)
        {
            await this.CurrentBucketExistsAsync();

            if (!await this.FileExistsAsync(filename))
            {
                return true;
            }

            await this.minioClient.RemoveObjectAsync(this.bucket, filename);

            var result = await this.eslasticRepository.DeleteDocumentAsync(filename);

            return result;
        }

        public async Task<bool> UploadFileAsync(IFormFile file)
        {
            await this.CurrentBucketExistsAsync();

            if (await this.FileExistsAsync(file.FileName))
            {
                throw this.logger.LogAndThrowException(ErrorTypes.ALREADY_EXISTS, new { File = file.FileName });
            }

            return await this.UploadDocumentAsync(file);
        }

        public async Task<bool> UpdateFileAsync(IFormFile file)
        {
            await this.CurrentBucketExistsAsync();

            if (!await this.FileExistsAsync(file.FileName))
            {
                throw this.logger.LogAndThrowException(ErrorTypes.NOT_FOUND, new { File = file.FileName });
            }

            return await this.UploadDocumentAsync(file);
        }

        public async Task<CustomAttachment> DownloadFileAsync(string filename)
        {
            await this.CurrentBucketExistsAsync();

            if (!await this.FileExistsAsync(filename))
            {
                throw this.logger.LogAndThrowException(ErrorTypes.NOT_FOUND, new { File = filename });
            }

            var fileMemoryStream = new MemoryStream();

            // Get file stream
            await this.minioClient.GetObjectAsync(
                this.bucket,
                filename,
                (stream) =>
                {
                    stream.CopyTo(fileMemoryStream);
                });

            // Get Extra info
            var result = (await this.eslasticRepository.SearchByNameAsync(filename)).FirstOrDefault();

            return new CustomAttachment()
            {
                AttachmentName = filename,
                AttachementType = result.Attachment.ContentType,
                AttachmentContent = fileMemoryStream,
            };
        }

        private async Task CurrentBucketExistsAsync()
        {
            if (string.IsNullOrWhiteSpace(this.bucket) || !await this.minioClient.BucketExistsAsync(this.bucket))
            {
                throw this.logger.LogAndThrowException(ErrorTypes.ERROR_BUCKET_DOESNT_EXISTS, new { Bucket = this.bucket });
            }
        }

        private async Task<bool> UploadDocumentAsync(IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                await this.minioClient.PutObjectAsync(this.bucket,
                    file.FileName,
                    stream,
                    stream.Length,
                    file.ContentType);
            }

            var result = await this.eslasticRepository.IndexDocumentAsync(file);

            return result;
        }
    }
}
