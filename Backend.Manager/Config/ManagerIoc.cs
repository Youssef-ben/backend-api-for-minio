namespace Backend.Manager.Config
{
    using System;
    using Backend.Manager.Implementation.Buckets;
    using Backend.Manager.Implementation.Uploader;
    using Backend.Manager.Repository;
    using Backend.Manager.Utils.Helpers.ConfigExtensions;
    using Backend.Manager.Utils.Models.ConfigModels;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Minio;
    using Nest;

    public static class ManagerIoc
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            // Get Configuration
            var config = configuration.GetConfigurationInstance<AppsettingsModel>("Settings");

            // Set the Client to be a singleton, since we need only one connection instance.
            services.AddSingleton<IElasticClient>(config.Elasticsearch.GetElasticSearchClient());
            services.AddSingleton<MinioClient>(config.Minio.GetMinioClient());

            services.AddTransient<IElasticsearchRepository, ElasticSearchRepository>();

            services.AddTransient<IBucketManager, BucketManager>();

            services.AddTransient<IUploaderManager, UploaderManager>();
        }

        public static ElasticClient GetElasticSearchClient(this BackendConfiguration self)
        {
            var connection = new ConnectionSettings(new Uri($"{self.NodeUri}:{self.Port}"));
            return new ElasticClient(connection);
        }

        public static MinioClient GetMinioClient(this BackendConfiguration self)
        {
            return new MinioClient($"{self.NodeUri}:{self.Port}", self.Username, self.Password);
        }
    }
}
