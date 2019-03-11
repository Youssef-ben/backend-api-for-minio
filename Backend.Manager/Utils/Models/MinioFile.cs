using System.IO;

namespace Backend.Manager.Utils.Models
{
    public class MinioFile
    {
        public MemoryStream Content { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }
    }
}
