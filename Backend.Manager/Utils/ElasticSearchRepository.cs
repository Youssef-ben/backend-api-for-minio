namespace Backend.Fileupload.Manager.Utils
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Backend.Fileupload.Manager.Utils.Models;
    using Backend.Manager.Helpers.Errors;
    using Backend.Manager.Helpers.Extension;
    using Backend.Manager.Utils.Models.ConfigModels;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Nest;

    public class ElasticSearchRepository
    {
        private readonly IElasticClient esClient;
        private readonly ILogger logger;
        private readonly ElasticsearchConfig config;

        public ElasticSearchRepository(ILogger<ElasticSearchRepository> logger, IOptions<AppsettingsModel> config, IElasticClient esClient)
        {
            this.logger = logger;
            this.config = config.Value.Elasticsearch;
            this.esClient = esClient;
            this.CreateEntityIndexIfNotExists();
            this.CreateAttachementPipeline();
        }

        public async Task<bool> IndexDocumentAsync(IFormFile document)
        {
            var base64File = string.Empty;
            using (var stream = new MemoryStream())
            {
                document.CopyTo(stream);
                base64File = Convert.ToBase64String(stream.ToArray());
            }

            var documentToIndex = new Document
            {
                Id = Guid.NewGuid().ToString(),
                Name = document.FileName,
                Content = base64File
            };

            var result = await this.esClient.IndexAsync(
               documentToIndex,
               i => i.Pipeline(this.config.PipelineIndex));

            if (!result.IsValid)
            {
                throw this.logger.LogAndThrowException(ErrorTypes.ERROR_WHILE_INDEXING_THE_DOCUMENT, new { result.DebugInformation });
            }

            return true;
        }

        private void CreateEntityIndexIfNotExists()
        {
            if (!this.esClient.IndexExists(this.config.DefaultIndex).Exists)
            {
                // Create an Index and Mapping for the Document POCO
                var indexResponse = this.esClient.CreateIndex(this.config.DefaultIndex, opt => opt
                    .Settings(s => s
                        .Analysis(a => a
                            .Analyzers(ad => ad
                                .Custom("windows_path_hierarchy_analyzer", ca => ca
                                    .Tokenizer("windows_path_hierarchy_tokenizer")))
                            .Tokenizers(t => t
                                .PathHierarchy("windows_path_hierarchy_tokenizer", ph => ph
                                    .Delimiter('\\')))))
                    .Mappings(m => m
                        .Map<Document>(mp => mp
                            .AutoMap()
                            .AllField(all => all
                                .Enabled(false))
                            .Properties(ps => ps
                                .Text(s => s
                                    .Name(n => n.Path)
                                    .Analyzer("windows_path_hierarchy_analyzer"))
                                .Object<Attachment>(a => a
                                    .Name(n => n.Attachment)
                                    .AutoMap())))));
            }
        }

        private void CreateAttachementPipeline()
        {
            if (this.esClient.GetPipeline(new GetPipelineRequest(this.config.DefaultIndex)).IsValid)
            {
                return;
            }

            var response = this.esClient.PutPipeline(this.config.DefaultIndex, p => p
                                .Description(this.config.PipelineDescription)
                                .Processors(pr => pr
                                    .Attachment<Document>(a => a
                                        .Field(f => f.Content)
                                        .TargetField(f => f.Attachment))
                                    .Remove<Document>(r => r
                                        .Field(f => f.Content))));
            if (!response.IsValid)
            {
                throw this.logger.LogAndThrowException(ErrorTypes.ERROR_WHILE_CREATING_ES_INGEST_PIPLINE, new { Es_Index = this.config.PipelineIndex });
            }
        }
    }
}
