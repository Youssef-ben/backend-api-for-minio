namespace Backend.API.Config
{
    using Newtonsoft.Json.Serialization;

    public class SnakeCaseContractResolver : DefaultContractResolver
    {
        public SnakeCaseContractResolver()
        {
            this.NamingStrategy = new SnakeCaseNamingStrategy();
        }
    }
}
