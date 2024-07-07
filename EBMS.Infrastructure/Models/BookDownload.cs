namespace EBMS.Infrastructure.Models
{
    public class BookDownload
    {
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
        public string BookUserId { get; set; } = null!;
        public BookUser BookUser { get; set; } = null!;
        public DateTime Downloaded_at { get; set; }
    }
}
