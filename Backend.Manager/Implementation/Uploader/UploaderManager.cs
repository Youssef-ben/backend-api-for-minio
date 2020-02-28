namespace Backend.Manager.Implementation.Uploader
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Backend.Manager.Helpers.Errors;
    using Backend.Manager.Helpers.Extension;
    using Backend.Manager.Implementation.Buckets;
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

        private readonly IBucketManager bucketManager;

        private readonly ILogger logger;

        private string bucket = string.Empty;

        public UploaderManager(ILogger<UploaderManager> logger, IOptions<AppsettingsModel> config, MinioClient minioClient, IBucketManager bucketManager)
        {
            this.logger = logger;
            this.configuration = config.Value.Minio;

            this.minioClient = minioClient;
            this.bucketManager = bucketManager;
        }

        public UploaderManager SetBucket(string name)
        {
            name = name.SanitizeString();

            this.bucket = string.IsNullOrWhiteSpace(name) ? this.configuration.DefaultIndex.ToLower() : name.ToLower();

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

        public async Task<ICollection<Item>> ListBucketFilesAsync(int page = 0, int size = 20)
        {
            size = size <= 0 ? 20 : size;
            page = page < 0 ? 0 : page;

            await this.CurrentBucketExistsAsync();

            return this.bucketManager
                .SetBucket(this.bucket)
                .GetListOfItemsForBucket()
                .Skip(size * page)
                .Take(size)
                .ToList();
        }

        public async Task<Item> GetFileAsync(string name)
        {
            await this.CurrentBucketExistsAsync();

            return this.bucketManager
                .SetBucket(this.bucket)
                .GetListOfItemsForBucket()
                .Where(f => f.Key == name)
                ?.FirstOrDefault();
        }

        public async Task<MinioFile> GetFileContentAsync(string name)
        {
            await this.CurrentBucketExistsAsync();

            var result = await this.GetFileAsync(name);

            if (result is null)
            {
                throw this.logger.LogAndThrowException(ErrorTypes.NOT_FOUND, new { File = name });
            }

            // TODO: Get file.
            return new MinioFile()
            {
                Name = result.Key,
                Content = result.Size.ToString(),
            };
        }

        public async Task<bool> RemoveFileAsync(string filename)
        {
            await this.CurrentBucketExistsAsync();

            if (!await this.FileExistsAsync(filename))
            {
                return true;
            }

            await this.minioClient.RemoveObjectAsync(this.bucket, filename);

            return true;
        }

        public async Task<Item> UploadFileAsync(IFormFile file)
        {
            await this.CurrentBucketExistsAsync();

            if (await this.FileExistsAsync(file.FileName))
            {
                throw this.logger.LogAndThrowException(ErrorTypes.ALREADY_EXISTS, new { File = file.FileName });
            }

            var result = await this.UploadDocumentAsync(file);
            if (!result)
            {
                throw this.logger.LogAndThrowException(ErrorTypes.ERROR_WHILE_INDEXING_THE_DOCUMENT, new { File = file.FileName });
            }

            return await this.GetFileAsync(file.FileName);
        }

        public async Task<Item> UpdateFileAsync(IFormFile file)
        {
            await this.CurrentBucketExistsAsync();

            if (!await this.FileExistsAsync(file.FileName))
            {
                throw this.logger.LogAndThrowException(ErrorTypes.NOT_FOUND, new { File = file.FileName });
            }

            var result = await this.UploadDocumentAsync(file);
            if (!result)
            {
                throw this.logger.LogAndThrowException(ErrorTypes.ERROR_WHILE_INDEXING_THE_DOCUMENT, new { File = file.FileName });
            }

            return await this.GetFileAsync(file.FileName);
        }

        public async Task<MinioFile> DownloadFileAsync(string filename)
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

            fileMemoryStream.Position = 0;

            return new MinioFile()
            {
                Name = filename,
                StreamContent = fileMemoryStream,
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

            return true;
        }
    }
}
