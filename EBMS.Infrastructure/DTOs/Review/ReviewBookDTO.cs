namespace EBMS.Infrastructure.DTOs.Review
{
    public class ReviewBookDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateOnly Published_at { get; set; }
    }
}
