namespace Backend.Tests.UnitTests.Minio
{
    using Backend.Manager.Helpers;
    using Backend.Manager.Helpers.Errors.CustomErrors;
    using Backend.Manager.Implementation.Buckets;
    using Backend.Manager.Repository;
    using Backend.Tests.Config;
    using Xunit;
    using Xunit.Priority;

    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class BucketTests
    {
        private readonly IBucketManager BucketManager;
        private readonly string BucketName = "test";
        private readonly string BucketNewName = "-new_test^%*()=+$#@-";

        public BucketTests()
        {
            var EsConfig = GetAppsettingsConfigs.GetConfiguration<ElasticSearchRepository>();
            var BucketManagerConfig = GetAppsettingsConfigs.GetConfiguration<BucketManager>();

            IElasticsearchRepository EsRepository = new ElasticSearchRepository(EsConfig.Logger, EsConfig.Configurations, EsConfig.Configurations.Value.Elasticsearch.GetElasticSearchClient());

            var minioClient = BucketManagerConfig.Configurations.Value.Minio.GetMinioClient();
            this.BucketManager = new BucketManager(BucketManagerConfig.Logger, BucketManagerConfig.Configurations, EsRepository, minioClient);

            this.BucketManager.SetBucket(this.BucketName);
        }

        [Fact]
        [Priority(1)]
        public async void Create_Bucket_Success()
        {
            var result = await this.BucketManager.CreateBucketAsync();
            Assert.True(result);
        }

        [Fact]
        [Priority(2)]
        public async void Rename_bucket_Success()
        {
            var result = await this.BucketManager.RenameBucketAsync(this.BucketNewName);
            Assert.True(result);
        }

        [Fact]
        [Priority(2)]
        public async void Rename_bucket_Bucket_Doesnt_Exists_Failed()
        {
            try
            {
                var result = await this.BucketManager.SetBucket("bucketDoesntExists").RenameBucketAsync(this.BucketNewName);
            }
            catch (ApplicationManagerException ex)
            {
                Assert.Contains(ex.Message, BackendConstants.ErrorMinioBucketDoesntExists);
            }
        }

        [Fact]
        [Priority(3)]
        public async void Get_Buckets_list_Success()
        {
            var result = await this.BucketManager.BucketsListAsync();
            Assert.True(result.Count > 0);
        }

        [Fact]
        [Priority(5)]
        public async void Delete_Bucket_Success()
        {
            var result = await this.BucketManager.DeleteBucketAsync();
            var result1 = await this.BucketManager.SetBucket(this.BucketNewName).DeleteBucketAsync();

            Assert.True(result);
            Assert.True(result1);
        }
    }
}
