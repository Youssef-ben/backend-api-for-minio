namespace Backend.Tests.UnitTests.Minio
{
    using System.Threading;
    using Backend.Manager.Config;
    using Backend.Manager.Helpers;
    using Backend.Manager.Helpers.Errors.CustomErrors;
    using Backend.Manager.Implementation.Buckets;
    using Backend.Manager.Implementation.Uploader;
    using Backend.Manager.Repository;
    using Backend.Tests.Config;
    using Xunit;
    using Xunit.Priority;

    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class UploaderTests
    {
        private readonly IBucketManager BucketManager;
        private readonly IUploaderManager UploaderManager;
        private readonly string BucketName = "uploader";
        private readonly string Filename = "test-file.pdf";

        public UploaderTests()
        {
            var EsConfig = GetAppsettingsConfigs.GetConfiguration<ElasticSearchRepository>();
            var UploaderManagerConfig = GetAppsettingsConfigs.GetConfiguration<UploaderManager>();
            var BucketManagerConfig = GetAppsettingsConfigs.GetConfiguration<BucketManager>();

            IElasticsearchRepository EsRepository = new ElasticSearchRepository(EsConfig.Logger, EsConfig.Configurations, EsConfig.Configurations.Value.Elasticsearch.GetElasticSearchClient());

            var minioClient = UploaderManagerConfig.Configurations.Value.Minio.GetMinioClient();
            this.UploaderManager = new UploaderManager(UploaderManagerConfig.Logger, UploaderManagerConfig.Configurations, EsRepository, minioClient);
            this.BucketManager = new BucketManager(BucketManagerConfig.Logger, UploaderManagerConfig.Configurations, EsRepository, minioClient);

            this.BucketManager.SetBucket(this.BucketName);
            this.UploaderManager.SetBucket(this.BucketName);
        }


        [Fact]
        [Priority(0)]
        public async void InitTests()
        {
            var result = await this.BucketManager.CreateBucketAsync();
            Assert.True(result);
        }

        [Fact]
        [Priority(1)]
        public async void Upload_file_Success()
        {
            var file = SharedMethods.MoqIFormFile(this.Filename);
            var result = await this.UploaderManager.UploadFileAsync(file);

            Assert.NotNull(result);

            // Used to wait for ElasticSearch to dispatch the new indexed document.
            Thread.Sleep(2000);
        }

        [Fact]
        [Priority(2)]
        public async void Upload_file_Failed()
        {
            var file = SharedMethods.MoqIFormFile(this.Filename);
            try
            {
                var result = await this.UploaderManager.UploadFileAsync(file);
            }
            catch (ApplicationManagerException ex)
            {
                Assert.Contains(ex.Message, BackendConstants.AlreadyExists);
            }
        }

        [Fact]
        [Priority(3)]
        public async void File_exists_Success()
        {
            var result = await this.UploaderManager.FileExistsAsync(this.Filename);
            Assert.True(result);
        }

        [Fact]
        [Priority(4)]
        public async void Download_File_Success()
        {
            var result = await this.UploaderManager.DownloadFileAsync(this.Filename);

            Assert.NotNull(result);
        }

        [Fact]
        [Priority(5)]
        public async void Get_File_Content_Success()
        {
            var result = await this.UploaderManager.GetFileContentAsync(this.Filename);

            Assert.NotNull(result);
            Assert.Contains("nest".ToLower(), result.Content.ToLower());
        }

        [Fact]
        [Priority(10)]
        public async void Remove_File_Success()
        {
            var result = await this.UploaderManager.RemoveFileAsync(this.Filename);
            Assert.True(result);

            result = await this.BucketManager.DeleteBucketAsync();
            Assert.True(result);
        }

    }
}
