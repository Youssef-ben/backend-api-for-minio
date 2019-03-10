namespace Backend.Tests.Config
{
    using System;
    using System.IO;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Internal;

    public static class SharedMethods
    {
        public static IFormFile MoqIFormFile(string Filename)
        {
            var startup = AppDomain.CurrentDomain.BaseDirectory;

            // Using a real file is the unit tests is not recommended. see Mock.
            var stream = new MemoryStream(File.ReadAllBytes(Path.Combine(startup, $"TestFiles/{Filename}")));

            IFormFile formFile = new FormFile(stream, 0, stream.Length, Filename.Split('.')[0], Filename)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };


            return formFile;
        }
    }
}
