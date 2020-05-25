using Minio.DataModel;

namespace Backend.Minio.Manager.Implementation.Buckets.Items
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Backend.Minio.Manager.Helpers;
    using Backend.Minio.Manager.Models;

    public interface IBucketItemsManager
    {
        Task<Item> UploadItemAsync(BucketItem item);

        Task<Item> UpdateItemAsync(BucketItem item);

        Task<MinioItem> DownloadItemAsync(BucketItem item);

        Task<Item> RemoveItemAsync(BucketItem item);

        Task<Item> GetItemDetailsAsync(BucketItem item);

        Task<ICollection<Item>> ListBucketItemsAsync(string bucket, int page = 1, int size = Constants.DEFAULT_PAGE_LIMITE);
    }
}
