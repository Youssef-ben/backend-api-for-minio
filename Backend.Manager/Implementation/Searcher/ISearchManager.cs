using System.Collections.Generic;
using System.Threading.Tasks;
using Minio.DataModel;

namespace Backend.Manager.Implementation.Searcher
{
    public interface ISearchManager
    {
        SearchManager SetBucket(string name);

        Task<ICollection<string>> AutoCompleteBucketsByNameAsync(string term, int page = 0, int size = 20);

        Task<ICollection<string>> AutoCompleteFilesByNameAsync(string term, int page = 0, int size = 20);

        Task<ICollection<Item>> SearchByNameAsync(string term, int page = 0, int size = 20);

        Task<ICollection<Item>> SearchByContentAsync(string term, int page = 0, int size = 20);
    }
}
