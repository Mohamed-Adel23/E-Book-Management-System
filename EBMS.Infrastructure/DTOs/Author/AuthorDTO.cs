namespace EBMS.Infrastructure.DTOs.Author
{
    public class AuthorDTO
    {
        public string? Message { get; set; }
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public byte[]? ProfilePic { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
    }
}
