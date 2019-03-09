namespace Backend.Manager.Repository
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Backend.Manager.Helpers.Errors;
    using Backend.Manager.Helpers.Extension;
    using Backend.Manager.Utils.Models;
    using Backend.Manager.Utils.Models.ConfigModels;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Nest;

    public class ElasticSearchRepository : IElasticsearchRepository
    {
        private readonly IElasticClient esClient;
        private readonly ILogger logger;
        private readonly BackendConfiguration config;

        private string bucketIndex = "-index";
        private string bucketPipelineIndex = "-attachments-index";

        public ElasticSearchRepository(ILogger<ElasticSearchRepository> logger, IOptions<AppsettingsModel> config, IElasticClient esClient)
        {
            this.logger = logger;
            this.config = config.Value.Elasticsearch;
            this.esClient = esClient;
        }

        public ElasticSearchRepository SetBucketIndex(string bucket)
        {
            bucket = string.IsNullOrWhiteSpace(bucket) ? this.config.DefaultIndex : bucket;

            this.bucketIndex = $"{bucket}-index".ToLower();
            this.bucketPipelineIndex = $"{bucket}-attachments-index".ToLower();

            return this;
        }

        public async Task<ICollection<Document>> SearchByNameAsync(string value, int limit = 25, int page = 0)
        {
            var result = await this.esClient
                .SearchAsync<Document>(s => s
                    .Index(this.bucketIndex)
                    .Type(nameof(Document).ToLower())
                    .Size(limit)
                    .Skip(page)
                    .Query(q => q
                        .Match(m => m
                           .Field(d => d.Name)
                           .Query(value))));

            if (!result.IsValid)
            {
                throw this.logger.LogAndThrowException(ErrorTypes.ERROR_WHILE_SEARCHING_FOR_VALUES, new { Es_Index = this.bucketIndex, Value = value });
            }

            return result.Documents?.ToList();
        }

        public async Task<ICollection<Document>> SearchByContentAsync(string value, int limit = 25, int page = 0)
        {
            var result = await this.esClient
               .SearchAsync<Document>(s => s
                   .Index(this.bucketIndex)
                   .Type(nameof(Document).ToLower())
                   .Size(limit)
                   .Skip(page)
                   .Query(q => q
                       .Match(m => m
                          .Field(d => d.Attachment.Content)
                          .Query(value))));

            if (!result.IsValid)
            {
                throw this.logger.LogAndThrowException(ErrorTypes.ERROR_WHILE_SEARCHING_FOR_VALUES, new { Es_Index = this.bucketIndex, Value = value });
            }

            return result.Documents?.ToList();
        }

        public async Task<ICollection<Document>> AutoCompleteAsync(string value, int limit = 25, int page = 0)
        {
            var suggestKey = $"{nameof(Document)}-suggest".ToLower();
            var nameSuggestion = $"{suggestKey}-{nameof(Document.Name).ToLower()}";

            var result = await this.esClient
                .SearchAsync<Document>(search => search
                    .Index(this.bucketIndex)
                    .Size(limit)
                    .Skip(page)
                    .Suggest(sg => sg
                        .Completion(nameSuggestion, comp => comp
                            .Field(x => x.NameCompletion)
                            .Prefix(value)
                            .SkipDuplicates(true)
                            .Fuzzy(fz => fz.Fuzziness(Fuzziness.Auto)))));

            if (!result.IsValid)
            {
                throw this.logger.LogAndThrowException(ErrorTypes.ERROR_WHILE_SEARCHING_FOR_VALUES, new { Description = result.ServerError.Error.CausedBy, Value = value });
            }

            if (result.Suggest.Count == 0)
            {
                return new List<Document>();
            }

            return result.Suggest[nameSuggestion]?
                .Select(sg => sg.Options)
                .Select(opt => opt.Select(src => src.Source))
                .FirstOrDefault()
                .ToList();
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
                Id = document.FileName,
                Name = document.FileName,
                NameCompletion = document.FileName,
                Content = base64File
            };

            var result = await this.esClient.IndexAsync(
               documentToIndex,
               i => i
               .Index(this.bucketIndex)
               .Pipeline(this.bucketPipelineIndex));

            if (!result.IsValid)
            {
                throw this.logger.LogAndThrowException(ErrorTypes.ERROR_WHILE_INDEXING_THE_DOCUMENT, new { result.DebugInformation });
            }

            return true;
        }

        public async Task<bool> RenameDocumentIndexAsync(string oldIndex, string newIndex, bool deleteOldIndex = true)
        {
            // Always set the Current Index to the Old one
            this.SetBucketIndex(oldIndex);
            oldIndex = this.bucketIndex;
            var ingestPipeline = this.bucketPipelineIndex;

            var isNotEmpty = await this.SearchByContentAsync(string.Empty);

            // Set the Current Index to the new Name.
            this.SetBucketIndex(newIndex);

            if ((await this.esClient.IndexExistsAsync(this.bucketIndex)).Exists)
            {
                return true;
            }

            if (isNotEmpty.Count > 0)
            {
                await this.ReIndexAsync(oldIndex);
            }
            else
            {
                await this.CreateIndexIfNotExists();
            }

            if (deleteOldIndex)
            {
                var isDeleted = await this.esClient.DeleteIndexAsync(oldIndex);
                var isPipeLIneDeleted = await this.esClient.DeletePipelineAsync(ingestPipeline);
                if (!isDeleted.IsValid || !isPipeLIneDeleted.IsValid)
                {
                    throw this.logger.LogAndThrowException(ErrorTypes.ERROR_WHILE_INDEXING_THE_DOCUMENT, new { isDeleted.DebugInformation });
                }
            }

            return true;
        }

        public async Task<bool> UpdateDocumentAsync(IFormFile document)
        {
            var results = await this.IndexDocumentAsync(document);

            if (!results)
            {
                throw this.logger.LogAndThrowException(ErrorTypes.ERROR_WHILE_UPDATING_THE_DOCUMENT, new { Description = "Couldn't update the document." });
            }

            return true;
        }

        public async Task<bool> DeleteDocumentAsync(string documentName)
        {
            var result = await this.esClient
                .DeleteAsync(DocumentPath<Document>
                .Id(documentName),
                u => u.Index(this.bucketIndex));

            if (!result.IsValid)
            {
                throw this.logger.LogAndThrowException(ErrorTypes.ERROR_WHILE_DELETING_THE_DOCUMENT, new { result.DebugInformation });
            }

            return true;
        }

        public async Task<bool> DocumentExistsAsync(string documentName)
        {
            var response = await this.esClient
                .DocumentExistsAsync(new DocumentExistsRequest(
                    this.bucketIndex,
                    nameof(Document).ToLower(),
                    documentName));

            if (!response.IsValid)
            {
                throw this.logger.LogAndThrowException(ErrorTypes.ERROR_WHILE_SEARCHING_FOR_VALUES, new { Es_Index = this.bucketIndex });
            }

            return response.Exists;
        }

        public async Task<bool> DeleteIndexAsync()
        {
            if (!(await this.esClient.IndexExistsAsync(this.bucketIndex)).Exists)
            {
                return true;
            }

            var response = await this.esClient.DeleteIndexAsync(this.bucketIndex);

            if (!response.IsValid)
            {
                throw this.logger.LogAndThrowException(ErrorTypes.ERROR_WHILE_DELETING_THE_INDEX, new { Es_Index = this.bucketIndex });
            }

            var pipelineResponse = await this.esClient.DeletePipelineAsync(this.bucketPipelineIndex);

            if (!pipelineResponse.IsValid)
            {
                throw this.logger.LogAndThrowException(ErrorTypes.ERROR_WHILE_DELETING_THE_INDEX, new { Es_Index = this.bucketPipelineIndex });
            }

            return true;
        }

        public async Task CreateIndexIfNotExists()
        {
            var response = await this.esClient.IndexExistsAsync(this.bucketIndex);
            if (!response.IsValid)
            {
                throw this.logger.LogAndThrowException(ErrorTypes.ERROR_WHILE_CREATING_ES_INDEX, new { Es_Index = this.bucketIndex });
            }

            if (!response.Exists)
            {
                // Create an Index and Mapping for the Document POCO
                var indexResponse = this.esClient.CreateIndex(this.bucketIndex, opt => opt
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

                if (!indexResponse.IsValid)
                {
                    throw this.logger.LogAndThrowException(ErrorTypes.ERROR_WHILE_CREATING_ES_INDEX, new { Es_Index = this.bucketIndex });
                }

                await this.CreateAttachementPipeline();
            }
        }

        private async Task CreateAttachementPipeline()
        {
            if ((await this.esClient.GetPipelineAsync(new GetPipelineRequest(this.bucketPipelineIndex))).IsValid)
            {
                return;
            }

            var response = this.esClient.PutPipeline(this.bucketPipelineIndex, p => p
                                .Description(this.config.PipelineDescription)
                                .Processors(pr => pr
                                    .Attachment<Document>(a => a
                                        .Field(f => f.Content)
                                        .TargetField(f => f.Attachment))
                                    .Remove<Document>(r => r
                                        .Field(f => f.Content))));
            if (!response.IsValid)
            {
                throw this.logger.LogAndThrowException(ErrorTypes.ERROR_WHILE_CREATING_ES_INGEST_PIPLINE, new { Es_Index = this.bucketPipelineIndex });
            }
        }

        private async Task ReIndexAsync(string oldIndex)
        {
            var result = await this.esClient.ReindexOnServerAsync(r => r
                .Source(s => s
                    .Index(oldIndex))
                .Destination(d => d
                    .Index(this.bucketIndex))
                .WaitForCompletion(true));

            if (!result.IsValid)
            {
                throw this.logger.LogAndThrowException(ErrorTypes.ERROR_WHILE_INDEXING_THE_DOCUMENT, new { result.DebugInformation });
            }

            await this.CreateAttachementPipeline();
        }

        private WildcardQuery GenerateWildCardQuery(string field, string value)
        {
            return new WildcardQuery()
            {
                Name = $"wildCardQuery_{field}",
                Boost = 1.1,
                Field = field.ToLower(),
                Value = $"*{value}*", // When adding `*` in the begining and ending of the value it add a hit to the elsaticSearch performance.
                Rewrite = MultiTermQueryRewrite.TopTermsBoost(10)
            };
        }
    }
}
