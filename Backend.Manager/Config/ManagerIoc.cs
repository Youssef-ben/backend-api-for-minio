namespace Backend.Manager.Config
{
    using Backend.Manager.Utils;
    using Microsoft.Extensions.DependencyInjection;

    public class ManagerIoc
    {
        public static void Register(IServiceCollection services)
        {
            services.AddTransient<IElasticsearchRepository, ElasticSearchRepository>();
        }
    }
}
