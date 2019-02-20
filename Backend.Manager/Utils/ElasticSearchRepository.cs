namespace Backend.Manager.Utils
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
    using Nest;

    public class ElasticSearchRepository : IElasticsearchRepository
    {
        private readonly IElasticClient esClient;
        private readonly ILogger logger;
        private readonly ElasticsearchConfig config;
        private readonly string bucketIndex = "_index";
        private readonly string bucketPipelineIndex = "_attachments_index";

        public ElasticSearchRepository(ILogger<ElasticSearchRepository> logger, ElasticsearchConfig config, IElasticClient esClient, string bucketName)
        {
            bucketName = string.IsNullOrWhiteSpace(bucketName) ? this.config.DefaultIndex : bucketName;

            this.bucketIndex = $"{bucketName}{this.bucketIndex}".ToLower();
            this.bucketPipelineIndex = $"{bucketName}{this.bucketPipelineIndex}".ToLower();

            this.logger = logger;
            this.config = config;
            this.esClient = esClient;

            this.CreateIndexIfNotExists();
            this.CreateAttachementPipeline();
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

        private void CreateIndexIfNotExists()
        {
            var response = this.esClient.IndexExists(this.bucketIndex);
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
            }
        }

        private void CreateAttachementPipeline()
        {
            if (this.esClient.GetPipeline(new GetPipelineRequest(this.bucketPipelineIndex)).IsValid)
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
