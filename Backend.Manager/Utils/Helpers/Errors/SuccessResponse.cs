using Minio.DataModel;

namespace Backend.Manager.Helpers.Errors
{
    public class SuccessResponse
    {
        public SuccessResponse()
        {
            this.Code = 200;
        }

        public int Error { get; set; }

        public int Code { get; set; }

        public string UserMessage { get; set; }

        public Bucket Bucket { get; set; }
    }
}
