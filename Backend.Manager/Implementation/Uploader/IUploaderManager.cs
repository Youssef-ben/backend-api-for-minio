using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Minio.Manager.Utils.Models;
using Microsoft.AspNetCore.Http;
using Minio.DataModel;

namespace Backend.Minio.Manager.Implementation.Uploader
{
    public interface IUploaderManager
    {
        UploaderManager SetBucket(string bucket);

        Task<Item> UploadFileAsync(IFormFile file);

        Task<Item> UpdateFileAsync(IFormFile file);

        Task<MinioFile> DownloadFileAsync(string filename);

        Task<bool> FileExistsAsync(string filename);

        Task<bool> RemoveFileAsync(string filename);

        Task<Item> GetFileAsync(string name);

        Task<MinioFile> GetFileContentAsync(string name);

        Task<ICollection<Item>> ListBucketFilesAsync(int page = 1, int size = 20);
    }
}
