namespace EBMS.Infrastructure.DTOs.Wishlist
{
    public class WishlistBookDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Desscription { get; set; }
        public string? CoverImage { get; set; }
        public decimal Rate { get; set; }
    }
}
