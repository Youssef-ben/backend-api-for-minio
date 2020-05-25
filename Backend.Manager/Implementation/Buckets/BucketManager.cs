using Minio;
using Minio.DataModel;

namespace Backend.Minio.Manager.Implementation.Buckets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using Backend.Minio.Manager.Helpers;
    using Backend.Minio.Manager.Helpers.Api.Response.Custom;
    using Backend.Minio.Manager.Helpers.Extension;
    using Backend.Minio.Manager.Utils.Helpers.Api.Response.Models;

    public class BucketManager : IBucketManager
    {
        private readonly MinioClient minioClient;

        public BucketManager(MinioClient minioClient)
        {
            this.minioClient = minioClient;
        }

        public async Task<bool> DoesBucketExistsAsync(string bucketName, bool throwError = false)
        {
            var result = await this.minioClient.BucketExistsAsync(bucketName.NormalizeString());

            if (throwError && !result)
            {
                var extras = new ExtrasDetails
                {
                    Manager = this.GetType().Name,
                    Field = "Bucket Name",
                    Value = bucketName,
                    Details = Constants.API_BUCKET_NOT_FOUND.FormatText(bucketName),
                };

                throw new ApplicationManagerException(extras.Details, Constants.API_BUCKET_NOT_FOUND_ID, extras);
            }

            return result;
        }

        public async Task<Bucket> CreateBucketAsync(string bucketName)
        {
            bucketName = bucketName.NormalizeString();

            if (await this.DoesBucketExistsAsync(bucketName))
            {
                var extras = new ExtrasDetails
                {
                    Manager = this.GetType().Name,
                    Field = "Bucket Name",
                    Value = bucketName,
                    Details = Constants.API_CANT_CREATE_BUCKET.FormatText(bucketName),
                };

                throw new ApplicationManagerException(extras.Details, Constants.API_CANT_CREATE_BUCKET_ID, extras);
            }

            await this.minioClient.MakeBucketAsync(bucketName);

            return await this.GetBucketAsync(bucketName);
        }

        public async Task<Bucket> GetBucketAsync(string bucketName)
        {
            bucketName = bucketName.NormalizeString();

            await this.DoesBucketExistsAsync(bucketName, true);

            var results = await this.GetAllBucketsAsync(1, Constants.MAX_BUCKETS_PER_PAGE);

            return results
                .Where(b => b.Name.Equals(bucketName))
                .FirstOrDefault();
        }

        public async Task<Bucket> RenameBucketAsync(string oldName, string newName)
        {
            oldName = oldName.NormalizeString();
            newName = newName.NormalizeString();

            // Check if the old bucket exists
            await this.DoesBucketExistsAsync(oldName, true);

            // Check that the new bucket doesn't exist.
            if (await this.DoesBucketExistsAsync(newName))
            {
                var extras = new ExtrasDetails
                {
                    Manager = this.GetType().Name,
                    Field = "Bucket Name",
                    Value = newName,
                    Details = Constants.API_CANT_UPDATE_BUCKET.FormatText(newName),
                };

                throw new ApplicationManagerException(extras.Details, Constants.API_CANT_UPDATE_BUCKET_ID, extras);
            }

            // Get the list of items from the first bucket.
            var originalBucketItems = await this.GetBucketListOfItemsAsync(oldName);

            // Create the New Bucket
            var newBucket = await this.CreateBucketAsync(newName);

            // Copy the items into the new one
            foreach (var item in originalBucketItems)
            {
                await this.minioClient.CopyObjectAsync(oldName, item.Key, newName);
            }

            // Delete the original one - Minio
            await this.DeleteBucketAsync(oldName);

            return newBucket;
        }

        public async Task<Bucket> DeleteBucketAsync(string bucketName)
        {
            bucketName = bucketName.NormalizeString();

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

        public async Task<ICollection<Bucket>> GetAllBucketsAsync(int page = 1, int size = Constants.DEFAULT_PAGE_LIMITE)
        {
            page = page <= 0 ? 1 : page;
            size = size <= 0 ? Constants.DEFAULT_PAGE_LIMITE : size;

            var result = await this.minioClient.ListBucketsAsync();

            return result?.Buckets
                .Skip(size * (page - 1))
                .Take(size)
                .ToList();
        }

        public async Task<ICollection<Item>> GetBucketListOfItemsAsync(string bucketName)
        {
            bucketName = bucketName.Normalize();

            await this.DoesBucketExistsAsync(bucketName, true);

            var bucketItems = new List<Item>();

            try
            {
                var observable = this.minioClient.ListObjectsAsync(bucketName);

                var subscription = observable.Subscribe(
                        item => bucketItems.Add(item),
                        ex => throw ex,
                        () => Console.WriteLine(Constants.LOG_MESSAGE.FormatText(bucketName)));

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
