namespace Backend.Manager.Repository
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Backend.Manager.Utils.Models;
    using Microsoft.AspNetCore.Http;

    public interface IElasticsearchRepository
    {
        ElasticSearchRepository SetBucketIndex(string bucket);

        Task<ICollection<Document>> SearchByNameAsync(string value, int limit = 25, int page = 0);

        Task<ICollection<Document>> SearchByContentAsync(string value, int limit = 25, int page = 0);

        Task<ICollection<Document>> AutoCompleteAsync(string value, int limit = 25, int page = 0);

        Task CreateIndexIfNotExists();

        Task<bool> DocumentExistsAsync(string documentName);

        Task<bool> IndexDocumentAsync(IFormFile document);

        Task<bool> RenameDocumentIndexAsync(string oldIndex, string newIndex, bool deleteOldIndex = true);

        Task<bool> UpdateDocumentAsync(IFormFile document);

        Task<bool> DeleteDocumentAsync(string documentName);

        Task<bool> DeleteIndexAsync();
    }
}
