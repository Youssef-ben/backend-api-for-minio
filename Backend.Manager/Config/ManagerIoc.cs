namespace Backend.Manager.Config
{
    using Backend.Manager.Repository;
    using Microsoft.Extensions.DependencyInjection;

    public static class ManagerIoc
    {
        public static void Register(IServiceCollection services)
        {
            services.AddTransient<IElasticsearchRepository, ElasticSearchRepository>();
        }
    }
}
