namespace Backend.Manager.Implementation.Buckets
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Minio.DataModel;

    public interface IBucketManager
    {
        BucketManager SetBucket(string name);

        /// <summary>
        /// The parameter is used to determine if we need to create ElasticSearch index or not.
        /// This means that when renaming the bucket we don't want to create the ElasticSearch index in this method but
        /// instead we will be creating it from the ElasticSearch layer.
        /// </summary>
        /// <param name="shouldCreateEsIndex">Define if we need to create the ElasticSearch Index too. </param>
        /// <returns>True if all went as expected, false otherwise.</returns>
        Task<bool> CreateBucketAsync(bool shouldCreateEsIndex = true);

        Task<Bucket> GetBucketAsync();

        Task<bool> RenameBucketAsync(string newBucketName);

        Task<bool> DeleteBucketAsync(string bucket = "");

        Task<bool> BucketExistsAsync();

        Task<ICollection<Bucket>> BucketsListAsync(int limit = 25, int page = 1);
    }
}
