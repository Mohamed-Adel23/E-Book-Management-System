using EBMS.Infrastructure.DTOs.Wishlist;
using EBMS.Infrastructure.Models;

namespace EBMS.Infrastructure.IServices
{
    public interface IWishlistService : IBaseService<Wishlist>
    {
        Task<WishlistDTO> AddToWishlistAsync(string curUserId, WishlistModel model);
        Task<IEnumerable<WishlistDTO>> GetUserWishlistAsync(string curUserId);
        Task<bool> RemoveFromWishlist(string curUserId, WishlistModel model);
    }
}
