namespace Backend.Manager.Utils.Models
{
    using Nest;

    public class Document
    {
        public string Id { get; set; }

        public string Name { get; set; }

        [Completion]
        public string NameCompletion { get; set; }

        public string Path { get; set; }

        public string Content { get; set; }

        public string LastModified { get; set; }

        public ulong Size { get; set; }

        public Attachment Attachment { get; set; }
    }
}
