namespace Backend.Manager.Implementation.Searcher
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Backend.Manager.Helpers.Extension;
    using Backend.Manager.Repository;
    using Backend.Manager.Utils.Models.ConfigModels;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Minio;
    using Minio.DataModel;

    public class SearchManager : ISearchManager
    {
        private readonly MinioClient minioClient;
        private readonly BackendConfiguration configuration;

        private readonly ILogger logger;
        private IElasticsearchRepository eslasticRepository;

        private string bucket = string.Empty;

        public SearchManager(
            ILogger<SearchManager> logger,
            IOptions<AppsettingsModel> config,
            IElasticsearchRepository elasticsearchRepository,
            MinioClient minioClient)
        {
            this.logger = logger;
            this.configuration = config.Value.Minio;

            this.eslasticRepository = elasticsearchRepository;
            this.minioClient = minioClient;
        }

        public SearchManager SetBucket(string name)
        {
            name = name.SanitizeString();

            this.bucket = string.IsNullOrWhiteSpace(name) ? this.configuration.DefaultIndex.ToLower() : name.ToLower();

            this.eslasticRepository = this.eslasticRepository.SetBucketIndex(this.bucket);

            return this;
        }

        // TODO : If we add a database support, we can search user buckets instead of all the buckets(which is not secure)
        public async Task<ICollection<string>> AutoCompleteBucketsByNameAsync(string term, int page = 0, int size = 20)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                term = string.Empty;
            }

            size = size <= 0 ? 20 : size;
            page = page < 0 ? 0 : page;

            var bucketsList = (await this.minioClient.ListBucketsAsync())?.Buckets;

            return bucketsList?
                    .Where(b => b.Name.ToLower().StartsWith(term.ToLower()))
                    .Select(b => b.Name)
                    .Skip(page)
                    .Take(size)
                    .ToList();
        }

        public async Task<ICollection<string>> AutoCompleteFilesByNameAsync(string term, int page = 0, int size = 20)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                term = string.Empty;
            }

            size = size <= 0 ? 20 : size;
            page = page < 0 ? 0 : page;

            var result = await this.eslasticRepository.AutoCompleteAsync(term, size, page);

            return result?.Select(p => p.Name).ToList();
        }

        public async Task<ICollection<Item>> SearchByNameAsync(string term, int page = 0, int size = 20)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                term = string.Empty;
            }

            size = size <= 0 ? 20 : size;
            page = page < 0 ? 0 : page;

            return (await this.eslasticRepository.SearchByNameAsync(term, size, page))
                ?.Select(f => new Item()
                {
                    Key = f.Name,
                    LastModified = f.LastModified,
                    Size = f.Size,
                }).ToList();
        }

        public async Task<ICollection<Item>> SearchByContentAsync(string term, int page = 0, int size = 20)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                term = string.Empty;
            }

            size = size <= 0 ? 20 : size;
            page = page < 0 ? 0 : page;

            return (await this.eslasticRepository.SearchByContentAsync(term, size, page))
                ?.Select(f => new Item()
                {
                    Key = f.Name,
                    LastModified = f.LastModified,
                    Size = f.Size,
                }).ToList();
        }
    }
}
