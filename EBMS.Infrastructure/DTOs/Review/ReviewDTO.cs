namespace EBMS.Infrastructure.DTOs.Review
{
    public class ReviewDTO : BaseDTO 
    {
        public int? Rate { get; set; }
        public string? Comment { get; set; }
        public DateTime? Created_at { get; set; }
        public DateTime? Updated_at { get; set; }

        public ReviewUserDTO? User { get; set; }
        public ReviewBookDTO? Book { get; set; }
    }
}
