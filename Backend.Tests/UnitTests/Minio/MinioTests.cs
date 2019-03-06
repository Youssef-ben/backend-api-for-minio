namespace Backend.Tests.UnitTests.Minio
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using global::Minio;
    using global::Minio.DataModel;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Internal;
    using Xunit;
    using Xunit.Priority;

    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class MinioTests
    {
        public MinioTests()
        {

        }

        [Fact]
        [Priority(1)]
        public void Create_Bucket_Success()
        {
        }

        [Fact]
        [Priority(2)]
        public void List_Bucket_Items_Success()
        {            
        }

        private IFormFile MoqIFormFile()
        {
            var Filename = "test-file.docx";
            var startup = AppDomain.CurrentDomain.BaseDirectory;

            // Using a real file is the unit tests is not recommended. see Mock.
            var stream = new MemoryStream(File.ReadAllBytes(Path.Combine(startup, $"TestFiles/{Filename}")));

            IFormFile formFile = new FormFile(stream, 0, stream.Length, Filename.Split('.')[0], Filename);
            return formFile;
        }
    }
}
