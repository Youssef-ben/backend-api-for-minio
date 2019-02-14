namespace Backend.Fileupload.Manager.Utils
{
    using Backend.Fileupload.Manager.Utils.Models;
    using Nest;

    public class Repository
    {
        private const string DEFAULT_INDEX = "documents_index";
        private const string PIPELINE_INDEX = "attachments_index";
        private readonly IElasticClient esClient;

        public Repository(IElasticClient esClient)
        {
            this.esClient = esClient;
            this.CreateEntityIndexIfNotExists();
            this.CreateAttachementPipeline();
        }

        private void CreateEntityIndexIfNotExists()
        {
            if (!this.esClient.IndexExists(DEFAULT_INDEX).Exists)
            {
                // Create an Index and Mapping for the Document POCO
                var indexResponse = this.esClient.CreateIndex(DEFAULT_INDEX, opt => opt
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
            var result = this.esClient.GetPipeline(new GetPipelineRequest(PIPELINE_INDEX));

            if (result.IsValid)
            {
                return;
            }

            var response = this.esClient.PutPipeline(PIPELINE_INDEX, p => p
                                .Description("Attachment pipeline for documents")
                                .Processors(pr => pr
                                    .Attachment<Document>(a => a
                                        .Field(f => f.Content)
                                        .TargetField(f => f.Attachment))
                                    .Remove<Document>(r => r
                                        .Field(f => f.Content))));
            if (!response.IsValid)
            {
                return;
            }
        }
    }
}
