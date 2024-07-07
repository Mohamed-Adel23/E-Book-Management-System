namespace EBMS.Infrastructure.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int Rate { get; set; }
        public string Comment { get; set; } = null!;
        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
        // Has One User
        public string UserId { get; set; } = null!;
        public BookUser BookUser { get; set; } = null!;
        // Has One Book
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
    }
}
