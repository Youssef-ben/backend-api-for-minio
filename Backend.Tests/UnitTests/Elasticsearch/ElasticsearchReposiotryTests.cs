namespace Backend.Tests.UnitTests.Elasticsearch
{
    using System;
    using System.IO;
    using System.Linq;
    using Backend.Manager.Repository;
    using Backend.Tests.Config;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Internal;
    using Xunit;
    using Xunit.Priority;

    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class ElasticsearchReposiotryTests
    {
        private readonly IElasticsearchRepository EsRepository;
        private readonly string Filename = "test-file.docx";
        private readonly string bucketName = "test";

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
            var file = this.MoqIFormFile();
            if (!await this.EsRepository.DocumentExistsAsync(this.Filename))
            {
                var result = await this.EsRepository.IndexDocumentAsync(file);
                Assert.True(result);
            }
        }

        [Fact]
        [Priority(2)]
        public async void Update_Document_Success()
        {
            var file = this.MoqIFormFile();
            var result = await this.EsRepository.UpdateDocumentAsync(file);
            Assert.True(result);
        }

        [Fact]
        [Priority(3)]
        public async void Search_By_Name_Success()
        {
            var result = await this.EsRepository.SearchByNameAsync(this.Filename.Split('.').FirstOrDefault());
            Assert.True(result.Count > 0);
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
                .SetBucketIndex("new_test")
                .RenameDocumentIndexAsync("test", false);

            Assert.True(result);
        }

        [Fact]
        [Priority(5)]
        public async void Delete_Document_Success()
        {
            var result = await this.EsRepository.DeleteDocumentAsync(this.Filename);
            Assert.True(result);
        }

        private IFormFile MoqIFormFile()
        {
            var startup = AppDomain.CurrentDomain.BaseDirectory;

            // Using a real file is the unit tests is not recommended. see Mock.
            var stream = new MemoryStream(File.ReadAllBytes(Path.Combine(startup, $"TestFiles/{Filename}")));

            IFormFile formFile = new FormFile(stream, 0, stream.Length, Filename.Split('.')[0], Filename);
            return formFile;
        }
    }
}
