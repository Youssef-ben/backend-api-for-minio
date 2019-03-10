using System.IO;

namespace Backend.Manager.Utils.Models
{
    public class CustomAttachment
    {
        public MemoryStream AttachmentContent { get; set; }

        public string AttachementType { get; set; }

        public string AttachmentName { get; set; }
    }
}
