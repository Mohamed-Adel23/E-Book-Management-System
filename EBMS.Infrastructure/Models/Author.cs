namespace EBMS.Infrastructure.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Bio { get; set; } = null!;
        public byte[]? ProfilePic { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }

        // Has Many Books
        public ICollection<Book>? Books { get; set; } = new List<Book>();
    }
}
