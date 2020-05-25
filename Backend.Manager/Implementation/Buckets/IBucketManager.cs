using Minio.DataModel;

namespace Backend.Minio.Manager.Implementation.Buckets
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Backend.Minio.Manager.Helpers;

    public interface IBucketManager
    {
        Task<bool> DoesBucketExistsAsync(string bucketName, bool throwError = false);

        Task<Bucket> CreateBucketAsync(string bucketName);

        Task<Bucket> GetBucketAsync(string bucketName);

        Task<Bucket> RenameBucketAsync(string oldName, string newName);

        Task<Bucket> DeleteBucketAsync(string bucketName);

        Task<ICollection<Item>> GetBucketListOfItemsAsync(string bucketName);

        Task<ICollection<Bucket>> GetAllBucketsAsync(int page = 1, int size = Constants.DEFAULT_PAGE_LIMITE);
    }
}
