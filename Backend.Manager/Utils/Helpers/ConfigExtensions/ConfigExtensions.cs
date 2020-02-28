using Backend.Manager.Utils.Models.ConfigModels;
using Microsoft.Extensions.Configuration;
using Minio;

namespace Backend.Manager.Utils.Helpers.ConfigExtensions
{
    public static class ConfigExtensions
    {
        public static TClass GetConfigurationInstance<TClass>(this IConfiguration self, string section)
            where TClass : class, new()
        {
            var instance = new TClass();
            self.Bind(section, instance);
            return instance;
        }

        public static MinioClient GetMinioClient(this BackendConfiguration self)
        {
            return new MinioClient($"{self.NodeUri}:{self.Port}", self.Username, self.Password);
        }
    }
}
