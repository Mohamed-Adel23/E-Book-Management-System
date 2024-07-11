using EBMS.Infrastructure.DTOs.Auth;
using EBMS.Infrastructure.DTOs.Book;
using Microsoft.AspNetCore.Http;

namespace EBMS.Infrastructure.IServices.IFile
{
    public interface IFileService
    {
        Task<UploadFile> UploadFileToServerAsync(IFormFile file, bool isFile, string folderPath);
        Task<UploadUserImage> UploadFileAsBytesAsync(IFormFile file);
        void DeleteFileFromServer(string fileName, string folderPath);
        Task<DownloadFile> DownloadFileAsync(string fileName, string folderPath);
    }
}
