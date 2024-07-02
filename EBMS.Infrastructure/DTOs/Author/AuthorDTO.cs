namespace EBMS.Infrastructure.DTOs.Author
{
    public class AuthorDTO : BaseDTO
    {
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public byte[]? ProfilePic { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
    }
}
