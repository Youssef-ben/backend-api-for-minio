namespace Backend.Minio.Manager.Ioc
{
    using Backend.Minio.Manager.Implementation.Buckets;
    using Backend.Minio.Manager.Implementation.Buckets.Items;
    using Microsoft.Extensions.DependencyInjection;

    public static class ManagerIoc
    {
        public static IServiceCollection AddManagerServices(this IServiceCollection self)
        {
            self
                .AddTransient<IBucketManager, BucketManager>()
                .AddTransient<IBucketItemsManager, BucketItemsManager>();

            return self;
        }
    }
}
