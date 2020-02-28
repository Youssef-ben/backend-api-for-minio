namespace Backend.Tests.UnitTests.Elasticsearch
{
    using System.Threading;
    using Backend.Manager.Config;
    using Backend.Manager.Repository;
    using Backend.Tests.Config;
    using Xunit;
    using Xunit.Priority;

    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class ElasticsearchReposiotryTests
    {
        private readonly IElasticsearchRepository EsRepository;
        private readonly string Filename = "test-file.pdf";
        private readonly string bucketName = "es-test";
        private readonly string bucketNewName = "es-new-test";

        public ElasticsearchReposiotryTests()
        {
            var config = GetAppsettingsConfigs.GetConfiguration<ElasticSearchRepository>();
            this.EsRepository = new ElasticSearchRepository(config.Logger, config.Configurations, config.Configurations.Value.Elasticsearch.GetElasticSearchClient());
            this.EsRepository.SetBucketIndex(this.bucketName);
        }

        [Fact]
        [Priority(0)]
        public async void INIT_INDEX()
        {
            await this.EsRepository.CreateIndexIfNotExists();
        }

        [Fact]
        [Priority(1)]
        public async void Index_Document_Success()
        {
            var file = SharedMethods.MoqIFormFile(this.Filename);
            if (!await this.EsRepository.DocumentExistsAsync(this.Filename))
            {
                var result = await this.EsRepository.IndexDocumentAsync(file);
                Assert.True(result);
            }

            // Used to wait for ElasticSearch to dispatch the new indexed document.
            Thread.Sleep(2000);
        }

        [Fact]
        [Priority(2)]
        public async void Update_Document_Success()
        {
            var file = SharedMethods.MoqIFormFile(this.Filename);
            var result = await this.EsRepository.UpdateDocumentAsync(file);
            Assert.True(result);
        }

        [Fact]
        [Priority(3)]
        public async void Search_By_Content_Success()
        {
            var result = await this.EsRepository.SearchByContentAsync("NEST");
            Assert.True(result.Count > 0);
        }

        [Fact]
        [Priority(3)]
        public async void Search_AutoComplete_Success()
        {
            var result = await this.EsRepository.AutoCompleteAsync("test");
            Assert.True(result.Count > 0);
        }

        [Fact]
        [Priority(4)]
        public async void Rename_Index_Success()
        {
            var result = await this.EsRepository
                .RenameIndexAsync(this.bucketName, this.bucketNewName, false);
            Assert.True(result);
        }

        [Fact]
        [Priority(6)]
        public async void Delete_Document_Success()
        {
            var result = await this.EsRepository.DeleteDocumentAsync(this.Filename);
            Assert.True(result);
        }

        [Fact]
        [Priority(10)]
        public async void Delete_Index_Success()
        {
            var result_TestIndex = await this.EsRepository
                .SetBucketIndex(this.bucketName)
                .DeleteIndexAsync();

            var result_new_testIndex = await this.EsRepository
                .SetBucketIndex(this.bucketNewName)
                .DeleteIndexAsync();

            Assert.True(result_TestIndex);
            Assert.True(result_new_testIndex);
        }
    }
}
