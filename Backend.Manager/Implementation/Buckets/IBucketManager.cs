using Minio.DataModel;

namespace Backend.Minio.Manager.Implementation.Buckets
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IBucketManager
    {
        const int DEFAULT_PAGE_LIMITE = 25;
        const int MAX_BUCKETS_PER_PAGE = 20000;

        Task<bool> BucketExistsAsync(string bucketName, bool throwError = false);

        Task<Bucket> CreateBucketAsync(string bucketName);

        Task<Bucket> GetBucketAsync(string bucketName);

        Task<Bucket> RenameBucketAsync(string oldName, string newName);

        Task<Bucket> DeleteBucketAsync(string bucketName);

        Task<ICollection<Item>> GetBucketListOfItemsAsync(string bucketName);

        Task<ICollection<Bucket>> GetAllBucketsAsync(int pageId = 1, int pageSize = DEFAULT_PAGE_LIMITE);
    }
}
