namespace Backend.Fileupload.Manager.Utils.Models
{
    using Nest;

    public class Document
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string Content { get; set; }

        public Attachment Attachment { get; set; }
    }
}
