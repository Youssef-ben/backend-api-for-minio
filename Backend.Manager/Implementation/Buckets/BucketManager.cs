using Minio;
using Minio.DataModel;

namespace Backend.Minio.Manager.Implementation.Buckets
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using Backend.Minio.Manager.Helpers;
    using Backend.Minio.Manager.Helpers.Api.Response.Custom;
    using Backend.Minio.Manager.Helpers.Extension;

    public class BucketManager : IBucketManager
    {
        private readonly MinioClient minioClient;

        public BucketManager(MinioClient minioClient)
        {
            this.minioClient = minioClient;
        }

        public async Task<bool> BucketExistsAsync(string bucketName)
        {
            bucketName = bucketName.SanitizeString();

            return await this.minioClient.BucketExistsAsync(bucketName);
        }

        public async Task<Bucket> CreateBucketAsync(string bucketName)
        {
            bucketName = bucketName.SanitizeString();

            if (await this.minioClient.BucketExistsAsync(bucketName))
            {
                throw new ApplicationManagerException(Constants.API_ALREADY_EXISTS.FormatText(bucketName), Constants.API_ALREADY_EXISTS_ID, new { Manager = this.GetType().Name, Field = bucketName });
            }

            await this.minioClient.MakeBucketAsync(bucketName);
            return await this.GetBucketAsync(bucketName);
        }

        public async Task<Bucket> GetBucketAsync(string bucketName)
        {
            bucketName = bucketName.SanitizeString();

            if (!await this.BucketExistsAsync(bucketName))
            {
                throw new ApplicationManagerException(Constants.API_NOT_FOUND.FormatText(bucketName), Constants.API_NOT_FOUND_ID, new { Manager = this.GetType().Name, Field = bucketName });
            }

            var results = await this.BucketsListAsync(1, IBucketManager.MAX_BUCKETS_PER_PAGE);

            return results
                .Where(b => b.Name.Equals(bucketName))
                .FirstOrDefault();
        }

        public async Task<Bucket> RenameBucketAsync(string oldName, string newName)
        {
            oldName = oldName.SanitizeString();
            newName = newName.SanitizeString();

            // Check if the old bucket exists
            if (!await this.BucketExistsAsync(oldName))
            {
                throw new ApplicationManagerException(Constants.API_NOT_FOUND.FormatText(oldName), Constants.API_NOT_FOUND_ID, new { Manager = this.GetType().Name, Field = oldName });
            }

            // Check that the new bucket doesn't exist.
            if (!await this.BucketExistsAsync(newName))
            {
                throw new ApplicationManagerException(Constants.API_ALREADY_EXISTS.FormatText(newName), Constants.API_ALREADY_EXISTS_ID, new { Manager = this.GetType().Name, Field = newName });
            }

            // Get the list of items from the first bucket.
            var originalBucketItems = await this.BucketsListAsync(1, IBucketManager.MAX_BUCKETS_PER_PAGE);

            // Create the New Bucket
            var bucket = await this.CreateBucketAsync(newName);

            // Copy the items into the new one
            foreach (var item in originalBucketItems)
            {
                await this.minioClient.CopyObjectAsync(oldName, item.Name, newName);
            }

            // Delete the original one - Minio
            await this.DeleteBucketAsync(oldName);

            return bucket;
        }

        public async Task<Bucket> DeleteBucketAsync(string bucketName)
        {
            bucketName = bucketName.SanitizeString();

            var bucket = await this.GetBucketAsync(bucketName);

            var bucketItems = await this.GetBucketListOfItemsAsync(bucketName);

            // Remove items.
            foreach (var item in bucketItems)
            {
                await this.minioClient.RemoveObjectAsync(bucketName, item.Key);
            }

            // Remove Bucket
            await this.minioClient.RemoveBucketAsync(bucketName);

            return bucket;
        }

        public async Task<ICollection<Bucket>> BucketsListAsync(int pageId = 1, int pageSize = IBucketManager.DEFAULT_PAGE_LIMITE)
        {
            pageId = pageId < 0 ? 1 : pageId;
            pageSize = pageSize <= 0 ? IBucketManager.DEFAULT_PAGE_LIMITE : pageSize;

            var result = await this.minioClient.ListBucketsAsync();

            return result?.Buckets
                .Skip(pageSize * (pageId - 1))
                .Take(pageSize)
                .ToList();
        }

        public async Task<ICollection<Item>> GetBucketListOfItemsAsync(string bucketName)
        {
            bucketName = bucketName.SanitizeString();

            if (!await this.BucketExistsAsync(bucketName))
            {
                throw new ApplicationManagerException(Constants.API_NOT_FOUND.FormatText(bucketName), Constants.API_NOT_FOUND_ID, new { Manager = this.GetType().Name, Field = bucketName });
            }

            var bucketItems = new List<Item>();

            try
            {
                var observable = this.minioClient.ListObjectsAsync(bucketName);

                var subscription = observable.Subscribe(
                        item => bucketItems.Add(item),
                        ex => throw ex,
                        () => Debug.WriteLine(Constants.LOG_MESSAGE.FormatText(bucketName)));

                observable.Wait();
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains(Constants.MINIO_API_ERROR))
                {
                    throw ex;
                }
            }

            return bucketItems;
        }
    }
}
