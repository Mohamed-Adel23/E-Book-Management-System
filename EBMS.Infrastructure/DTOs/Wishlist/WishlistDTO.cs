namespace EBMS.Infrastructure.DTOs.Wishlist
{
    public class WishlistDTO
    {
        public string? Message { get; set; }
        public string? UserName { get; set; }
        public WishlistBookDTO? BookData { get; set; }
    }
}
