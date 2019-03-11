namespace Backend.Tests.UnitTests.Minio
{
    using System.Threading;
    using Backend.Manager.Config;
    using Backend.Manager.Implementation.Buckets;
    using Backend.Manager.Implementation.Searcher;
    using Backend.Manager.Implementation.Uploader;
    using Backend.Manager.Repository;
    using Backend.Tests.Config;
    using Xunit;
    using Xunit.Priority;

    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class SearcherTests
    {
        private readonly IBucketManager BucketManager;
        private readonly IUploaderManager UploaderManager;
        private readonly ISearchManager SearcherManager;

        private readonly string BucketName = "searcher";
        private readonly string Filename = "test-file.pdf";

        public SearcherTests()
        {
            var EsConfig = GetAppsettingsConfigs.GetConfiguration<ElasticSearchRepository>();
            var UploaderManagerConfig = GetAppsettingsConfigs.GetConfiguration<UploaderManager>();
            var BucketManagerConfig = GetAppsettingsConfigs.GetConfiguration<BucketManager>();
            var SearcherManagerConfig = GetAppsettingsConfigs.GetConfiguration<SearchManager>();

            IElasticsearchRepository EsRepository = new ElasticSearchRepository(EsConfig.Logger, EsConfig.Configurations, EsConfig.Configurations.Value.Elasticsearch.GetElasticSearchClient());

            var minioClient = UploaderManagerConfig.Configurations.Value.Minio.GetMinioClient();
            this.UploaderManager = new UploaderManager(UploaderManagerConfig.Logger, UploaderManagerConfig.Configurations, EsRepository, minioClient);
            this.BucketManager = new BucketManager(BucketManagerConfig.Logger, UploaderManagerConfig.Configurations, EsRepository, minioClient);
            this.SearcherManager = new SearchManager(SearcherManagerConfig.Logger, SearcherManagerConfig.Configurations, EsRepository, minioClient);

            this.BucketManager.SetBucket(this.BucketName);
            this.UploaderManager.SetBucket(this.BucketName);
            this.SearcherManager.SetBucket(this.BucketName);
        }

        [Fact]
        [Priority(0)]
        public async void Init_Values()
        {
            var result = await this.BucketManager.CreateBucketAsync();
            Assert.True(result);

            var file = SharedMethods.MoqIFormFile(this.Filename);
            if (await this.UploaderManager.FileExistsAsync(this.Filename))
            {
                Assert.True(true);
                return;
            }

            var item = await this.UploaderManager.UploadFileAsync(file);

            Assert.NotNull(item);

            // Used to wait for ElasticSearch to dispatch the new indexed document.
            Thread.Sleep(2000);
        }

        [Fact]
        [Priority(1)]
        public async void Autocomplete_Bucket_Names_Success()
        {
            var result = await this.SearcherManager.AutoCompleteBucketsByNameAsync("sear");

            Assert.True(result.Count > 0);
        }

        [Fact]
        [Priority(2)]
        public async void Autocomplete_File_Names_Success()
        {
            var result = await this.SearcherManager.AutoCompleteFilesByNameAsync("test");

            Assert.True(result.Count > 0);
        }

        [Fact]
        [Priority(3)]
        public async void Search_Files_By_Name_Success()
        {
            var result = await this.SearcherManager.SearchByNameAsync("test");

            Assert.True(result.Count > 0);
        }


        [Fact]
        [Priority(4)]
        public async void Search_Files_By_Content_Success()
        {
            var result = await this.SearcherManager.SearchByContentAsync("NEST");

            Assert.True(result.Count > 0);
        }

        [Fact]
        [Priority(10)]
        public async void Delete_Search_Bucket_Success()
        {
            var result = await this.BucketManager.DeleteBucketAsync();

            Assert.True(result);
        }
    }
}
