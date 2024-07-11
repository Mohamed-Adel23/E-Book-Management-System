using EBMS.Infrastructure.DTOs.Auth;
using EBMS.Infrastructure.DTOs.Author;
using EBMS.Infrastructure.DTOs.Book;
using EBMS.Infrastructure.IServices.IFile;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace EBMS.Data.Services.File
{
    public class FileService : IFileService
    {
        private string[] _allowedFileExtensions = { ".pdf", ".txt", ".doc", ".docx" };
        private int _maxFileSize = 1 * 1024 * 1024 * 1024; // 1 GB
        private string[] _allowedImageExtensions = { ".jpg", ".png", ".jpeg" };
        private int _maxImageSize = 3 * 1024 * 1024; // 3 MB

        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<UploadFile> UploadFileToServerAsync(IFormFile file, bool isFile, string folderPath)
        {
            var result = new UploadFile();
            // Check The Extension 
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (isFile)
            {
                if (!_allowedFileExtensions.Contains(fileExtension))
                {
                    result.Message = $"Invalid Book File Extension, Allowed Ones are ({string.Join(",", _allowedFileExtensions)})";
                    return result;
                }
                // Check The Size 
                if (file.Length > _maxFileSize)
                {
                    result.Message = $"File Size Exceeds The Size Limit, You should upload files with size less than or equal to {_maxFileSize / (1024 * 1024 * 1024)}GB";
                    return result;
                }
            }
            else
            {
                if (!_allowedImageExtensions.Contains(fileExtension))
                {
                    result.Message = $"Invalid Book Image Extension, Allowed Ones are ({string.Join(",", _allowedImageExtensions)})";
                    return result;
                }
                // Check The Size 
                if (file.Length > _maxImageSize)
                {
                    result.Message = $"File Size Exceeds The Size Limit, You should upload files with size less than or equal to {_maxImageSize / (1024 * 1024 * 1024)}GB";
                    return result;
                }
            }

            var fileName = file.FileName;
            // Create New Fake Name for the file
            fileName = $"{Guid.NewGuid().ToString().Substring(0, 15)}-EBMS{Path.GetExtension(fileName).ToLowerInvariant()}";
            // Get The Actual Path to store on server
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, folderPath, fileName);
            // Move The File
            using var fileStream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fileStream);

            result.FileName = fileName;

            return result;
        }

        public async Task<UploadUserImage> UploadFileAsBytesAsync(IFormFile file)
        {
            var result = new UploadUserImage();

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedImageExtensions.Contains(fileExtension))
            {
                result.Message = $"Profile Image should have one of the following extensions {string.Join(",", _allowedImageExtensions)}";
                return result;
            }
            if (file.Length > _maxImageSize)
            {
                result.Message = $"File exceeds size limit: {_maxImageSize / (1024 * 1024)}M, choose another one with less size!";
                return result;
            }
            using var dataStream = new MemoryStream();
            await file.CopyToAsync(dataStream);

            result.MemoryStream = dataStream;

            return result;
        }

        public void DeleteFileFromServer(string fileName, string folderPath)
        {
            try
            {
                // Remove the Old File From Server
                var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, folderPath, fileName);
                Thread.Sleep(1000);
                System.IO.File.Delete(oldFilePath);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<DownloadFile> DownloadFileAsync(string fileName, string folderPath)
        {
            var result = new DownloadFile();

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, folderPath, fileName);
            MemoryStream memoryStream = new();
            using var fileStream = new FileStream(filePath, FileMode.Open);
            await fileStream.CopyToAsync(memoryStream);

            memoryStream.Position = 0;

            var fileExtension = Path.GetExtension(filePath).ToLowerInvariant();
            result.FileName = filePath;
            result.MemoryDataStream = memoryStream;
            result.ContentType = GetContentType(fileExtension);

            return result;
        }



        private string GetContentType(string fileExt) =>
            fileExt switch
            {
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                ".doc" => "application/vnd.ms-word",
                "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
    }
}
