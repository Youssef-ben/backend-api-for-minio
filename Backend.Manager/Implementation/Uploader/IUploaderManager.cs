using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Manager.Utils.Models;
using Microsoft.AspNetCore.Http;
using Minio.DataModel;

namespace Backend.Manager.Implementation.Uploader
{
    public interface IUploaderManager
    {
        UploaderManager SetBucket(string bucket);

        Task<bool> UploadFileAsync(IFormFile file);

        Task<CustomAttachment> DownloadFileAsync(string filename);

        Task<bool> UpdateFileAsync(IFormFile file);

        Task<bool> FileExistsAsync(string filename);

        Task<bool> RemoveFileAsync(string filename);

        Task<ICollection<Item>> ListBucketFilesAsync();
    }
}
