namespace Backend.Manager.Implementation.Buckets
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Minio.DataModel;

    public interface IBucketManager
    {
        BucketManager SetBucket(string name);

        Task<bool> CreateBucketAsync();

        Task<bool> RenameBucketAsync(string newName);

        Task<bool> DeleteBucketAsync();

        Task<bool> BucketExistsAsync();

        Task<ICollection<Bucket>> BucketsListAsync(int limit = 25, int page = 1);
    }
}
