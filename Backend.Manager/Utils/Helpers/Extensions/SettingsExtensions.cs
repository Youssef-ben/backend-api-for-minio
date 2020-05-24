using Minio;

namespace Backend.Minio.Manager.Utils.Helpers.ConfigExtensions
{
    using Backend.Minio.Manager.Utils.Models.ConfigModels;
    using Microsoft.Extensions.Configuration;

    public static class SettingsExtensions
    {
        public static TSettingsClass GetSettings<TSettingsClass>(this IConfiguration self)
            where TSettingsClass : class, new()
        {
            var section = typeof(TSettingsClass).Name.Replace("Model", string.Empty);

            var instance = new TSettingsClass();
            self.Bind(section, instance);
            return instance;
        }

        public static MinioClient GetMinioClient(this MinioSettings self)
        {
            return new MinioClient($"{self.NodeUri}:{self.Port}", self.Username, self.Password);
        }
    }
}
