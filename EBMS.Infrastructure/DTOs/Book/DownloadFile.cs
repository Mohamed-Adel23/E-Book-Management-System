namespace EBMS.Infrastructure.DTOs.Book
{
    public class DownloadFile
    {
        public string? Message { get; set; }

        public MemoryStream MemoryDataStream { get; set; }

        public string ContentType { get; set; }

        public string FileName { get; set; }
    }
}
