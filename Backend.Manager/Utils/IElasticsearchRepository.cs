namespace Backend.Manager.Utils
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Backend.Manager.Utils.Models;
    using Microsoft.AspNetCore.Http;

    public interface IElasticsearchRepository
    {
        Task<ICollection<Document>> SearchByNameAsync(string value, int limit = 25, int page = 0);

        Task<ICollection<Document>> SearchByContentAsync(string value, int limit = 25, int page = 0);

        Task<ICollection<Document>> AutoCompleteAsync(string value, int limit = 25, int page = 0);

        Task<bool> DocumentExistsAsync(string documentName);

        Task<bool> IndexDocumentAsync(IFormFile document);

        Task<bool> UpdateDocumentAsync(IFormFile document);

        Task<bool> DeleteDocumentAsync(string documentName);
    }
}
