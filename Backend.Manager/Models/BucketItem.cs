namespace Backend.Minio.Manager.Models
{
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    public class BucketItem
    {
        public string Bucket { get; set; }

        public string ItemKey { get; set; }

        [JsonIgnore]
        public IFormFile FormFile { get; set; }
    }
}
