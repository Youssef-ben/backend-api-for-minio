using Minio;
using Minio.DataModel;
using Minio.Exceptions;

namespace Backend.Minio.Manager.Implementation.Buckets.Items
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Backend.Minio.Manager.Helpers;
    using Backend.Minio.Manager.Helpers.Api.Response.Custom;
    using Backend.Minio.Manager.Helpers.Extension;
    using Backend.Minio.Manager.Implementation.Buckets;
    using Backend.Minio.Manager.Models;
    using Backend.Minio.Manager.Utils.Helpers.Api.Response.Models;

    public class BucketItemsManager : IBucketItemsManager
    {
        private readonly MinioClient minioClient;

        private readonly IBucketManager bucketManager;

        public BucketItemsManager(MinioClient minioClient, IBucketManager bucketManager)
        {
            this.minioClient = minioClient;
            this.bucketManager = bucketManager;
        }

        public async Task<Item> UploadItemAsync(BucketItem item)
        {
            item.Bucket = item.Bucket.NormalizeString();

            if (await this.DoesItemExistsAsync(item, false) != null)
            {
                var extras = new ExtrasDetails
                {
                    Manager = this.GetType().Name,
                    Field = "Bucket Item",
                    Value = item.ItemKey,
                    Details = Constants.API_ITEM_ALREADY_EXISTS.FormatText(item.ItemKey, item.Bucket),
                };

                throw new ApplicationManagerException(extras.Details, Constants.API_ITEM_ALREADY_EXISTS_ID, extras);
            }

            await this.UploadDocumentAsync(item);

            return await this.GetItemDetailsAsync(item);
        }

        public async Task<Item> UpdateItemAsync(BucketItem item)
        {
            item.Bucket = item.Bucket.NormalizeString();

            await this.DoesItemExistsAsync(item, true);

            await this.UploadDocumentAsync(item);

            return await this.GetItemDetailsAsync(item);
        }

        public async Task<MinioItem> DownloadItemAsync(BucketItem item)
        {
            item.Bucket = item.Bucket.NormalizeString();

            await this.DoesItemExistsAsync(item, true);

            var fileMemoryStream = new MemoryStream();

            // Get file stream
            await this.minioClient.GetObjectAsync(
                item.Bucket,
                item.ItemKey,
                (stream) =>
                {
                    stream.CopyTo(fileMemoryStream);
                });

            fileMemoryStream.Position = 0;

            var state = await this.GetObjectStatAsync(item);

            return new MinioItem()
            {
                Name = state.ObjectName,
                Type = state.ContentType,
                StreamContent = fileMemoryStream,
            };
        }

        public async Task<Item> GetItemDetailsAsync(BucketItem item)
        {
            item.Bucket = item.Bucket.NormalizeString();

            await this.DoesItemExistsAsync(item);

            var result = await this.bucketManager.GetBucketListOfItemsAsync(item.Bucket);

            return result?
                .FirstOrDefault(x => x.Key == item.ItemKey);
        }

        public async Task<Item> RemoveItemAsync(BucketItem item)
        {
            item.Bucket = item.Bucket.NormalizeString();

            var result = await this.DoesItemExistsAsync(item, true);

            await this.minioClient.RemoveObjectAsync(item.Bucket, item.ItemKey);

            return result;
        }

        public async Task<ICollection<Item>> ListBucketItemsAsync(string bucket, int page = 1, int size = Constants.DEFAULT_PAGE_LIMITE)
        {
            bucket = bucket.NormalizeString();

            page = page <= 0 ? 1 : page;
            size = size <= 0 ? Constants.DEFAULT_PAGE_LIMITE : size;

            var result = await this.bucketManager.GetBucketListOfItemsAsync(bucket);

            return result?
                .Skip(size * (page - 1))
                .Take(size)
                .ToList();
        }

        private async Task<Item> DoesItemExistsAsync(BucketItem item, bool throwError = false)
        {
            await this.bucketManager.DoesBucketExistsAsync(item.Bucket, true);

            try
            {
                var result = await this.minioClient.StatObjectAsync(item.Bucket, item.ItemKey);

                return this.ConvertObjectStatToItem(result);
            }
            catch (ObjectNotFoundException)
            {
                if (throwError)
                {
                    var extra = new ExtrasDetails
                    {
                        Manager = this.GetType().Name,
                        Field = "Bucket Item",
                        Value = item.ItemKey,
                        Details = Constants.API_ITEM_NOT_FOUND.FormatText(item.ItemKey, item.Bucket),
                    };

                    throw new ApplicationManagerException(extra.Details, Constants.API_ITEM_NOT_FOUND_ID, extra);
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task UploadDocumentAsync(BucketItem item)
        {
            using var stream = item.FormFile.OpenReadStream();

            await this.minioClient.PutObjectAsync(
                item.Bucket,
                item.FormFile.FileName,
                stream,
                stream.Length,
                item.FormFile.ContentType);
        }

        private Item ConvertObjectStatToItem(ObjectStat stat)
        {
            return new Item
            {
                ETag = stat.ETag,
                Key = stat.ObjectName,
                Size = (ulong)stat.Size,
                IsDir = false,
                LastModified = stat.LastModified.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
            };
        }

        /// <summary>
        /// Note: This method should only be called after you use <see cref="DoesItemExistsAsync(BucketItem, bool)"/>.
        /// </summary>
        /// <param name="item">The Bucket item.</param>
        /// <returns>Object State.</returns>
        private async Task<ObjectStat> GetObjectStatAsync(BucketItem item)
        {
            return await this.minioClient.StatObjectAsync(item.Bucket, item.ItemKey);
        }
    }
}
