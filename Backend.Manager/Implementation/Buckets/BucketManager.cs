namespace Backend.Manager.Implementation.Buckets
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Backend.Manager.Helpers.Errors;
    using Backend.Manager.Helpers.Extension;
    using Backend.Manager.Repository;
    using Backend.Manager.Utils.Models.ConfigModels;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Minio;
    using Minio.DataModel;

    public class BucketManager : IBucketManager
    {
        private readonly MinioClient minioClient;
        private readonly BackendConfiguration configuration;

        private readonly ILogger logger;
        private IElasticsearchRepository esRepository;

        private string bucket = string.Empty;

        public BucketManager(ILogger<BucketManager> logger, IOptions<AppsettingsModel> config, IElasticsearchRepository elasticsearchRepository, MinioClient minioClient)
        {
            this.logger = logger;
            this.configuration = config.Value.Minio;

            this.esRepository = elasticsearchRepository;
            this.minioClient = minioClient;
        }

        public BucketManager SetBucket(string name)
        {
            name = name.SanitizeString();

            this.bucket = string.IsNullOrWhiteSpace(name) ? this.configuration.DefaultIndex.ToLower() : name.ToLower();

            this.esRepository = this.esRepository.SetBucketIndex(this.bucket);

            return this;
        }

        public async Task<bool> BucketExistsAsync()
        {
            return await this.minioClient.BucketExistsAsync(this.bucket);
        }

        /// <summary>
        /// Create a new Bucket and Elasticsearch Index.
        /// The parameter is used to determine if we need to create ElasticSearch index or not.
        /// This means that when renaming the bucket we don't want to create the ElasticSearch index in this method but
        /// instead we will be creating it from the ElasticSearch layer.
        /// </summary>
        /// <param name="shouldCreateEsIndex">Define if we need to create the ElasticSearch Index too. </param>
        /// <returns>True if all went as expected, false otherwise.</returns>
        public async Task<bool> CreateBucketAsync(bool shouldCreateEsIndex = true)
        {
            if (!await this.minioClient.BucketExistsAsync(this.bucket))
            {
                await this.minioClient.MakeBucketAsync(this.bucket);
            }

            if (shouldCreateEsIndex)
            {
                await this.esRepository.CreateIndexIfNotExists();
            }

            return true;
        }

        public async Task<bool> RenameBucketAsync(string newBucketName)
        {
            newBucketName = newBucketName.SanitizeString();

            // Keep the name of the old bucket
            var oldBucketName = this.bucket;

            // Check if the old bucket exists
            if (!await this.minioClient.BucketExistsAsync(this.bucket))
            {
                throw this.logger.LogAndThrowException(ErrorTypes.ERROR_BUCKET_DOESNT_EXISTS, new { Bucket = this.bucket });
            }

            // Get the list of items from the first bucket
            var originalBucketObjects = await this.minioClient.GetBucketItemsAsync(oldBucketName).ConfigureAwait(false);

            // set the Name of the new Bucket
            this.SetBucket(newBucketName);

            // Create the New Bucket
            if (!await this.CreateBucketAsync(false))
            {
                throw this.logger.LogAndThrowException(ErrorTypes.ERROR_WHILE_CREATING_MINIO_BUCKET, new { Bucket = newBucketName });
            }

            // Copy the items into the new one - Minio
            foreach (var item in originalBucketObjects)
            {
                await this.minioClient.CopyObjectAsync(oldBucketName, item.Key, this.bucket);
            }

            // Delete the original one - Minio
            await this.DeleteBucketAsync(oldBucketName);

            // Re-index in elasticsearch.
            var result = await this.esRepository.RenameIndexAsync(oldBucketName, newBucketName);

            return result;
        }

        public async Task<bool> DeleteBucketAsync(string bucket = "")
        {
            if (string.IsNullOrWhiteSpace(bucket))
            {
                bucket = this.bucket;
            }

            bucket = bucket.SanitizeString();

            if (!await this.minioClient.BucketExistsAsync(bucket))
            {
                return true;
            }

            var bucketObjects = await this.minioClient.GetBucketItemsAsync(bucket).ConfigureAwait(false);
            foreach (var item in bucketObjects)
            {
                await this.minioClient.RemoveObjectAsync(bucket, item.Key);
            }

            await this.minioClient.RemoveBucketAsync(bucket);

            /**
             * Delete the Elasticsearch index only when the current bucket name equals the param.
             * This means that the method was called by a method other that the <see cref="RenameBucketAsync(string)"/>
             */
            if (this.bucket.Equals(bucket))
            {
                await this.esRepository.DeleteIndexAsync();
            }

            return true;
        }

        public async Task<ICollection<Bucket>> BucketsListAsync(int limit = 25, int page = 1)
        {
            limit = limit <= 0 ? 25 : limit;
            page = page < 1 ? 1 : page;

            var result = await this.minioClient.ListBucketsAsync();

            return result?.Buckets;
        }
    }
}
