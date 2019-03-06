namespace Backend.Manager.Implementation.Buckets
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Backend.Manager.Repository;
    using Backend.Manager.Utils.Models.ConfigModels;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Minio;
    using Minio.DataModel;

    public class BucketManager : IBucketManager
    {
        private readonly MinioClient minioClient;
        private readonly ILogger logger;
        private readonly BackendConfiguration configuration;

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
            this.bucket = string.IsNullOrWhiteSpace(name) ? this.configuration.DefaultIndex.ToLower() : name.ToLower();

            this.esRepository = this.esRepository.SetBucketIndex(this.bucket);

            return this;
        }

        public async Task<bool> BucketExistsAsync()
        {
            return await this.minioClient.BucketExistsAsync(this.bucket);
        }

        public async Task<bool> CreateBucketAsync()
        {
            await this.minioClient.MakeBucketAsync(this.bucket);

            await this.esRepository.CreateIndexIfNotExists();

            return true;
        }

        public async Task<bool> RenameBucketAsync(string newName)
        {
            // Keep the name of the old bucket
            var oldBucketName = this.bucket;

            // Get the list of items from the first bucket
            var originalBucketObjects = await this.minioClient.GetBucketItemsAsync(oldBucketName).ConfigureAwait(false);

            // set the Name of the new Bucket
            this.SetBucket(newName);

            // Copy the items into the new one - Minio
            foreach (var item in originalBucketObjects)
            {
                await this.minioClient.CopyObjectAsync(oldBucketName, item.Key, this.bucket);
            }

            // Delete the original one - Minio
            await this.minioClient.RemoveBucketAsync(oldBucketName);

            // Re-index in elasticsearch.
            await this.esRepository.RenameDocumentIndexAsync(oldBucketName);

            throw new NotImplementedException();
        }

        public Task<bool> DeleteBucketAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<ICollection<Bucket>> BucketsListAsync(int limit = 25, int page = 1)
        {
            throw new System.NotImplementedException();
        }
    }
}
