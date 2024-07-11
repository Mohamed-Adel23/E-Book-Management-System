using EBMS.Data.DataAccess;
using EBMS.Infrastructure.DTOs.Wishlist;
using EBMS.Infrastructure.IServices;
using EBMS.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EBMS.Data.Services
{
    public class WishlistService : BaseService<Wishlist>, IWishlistService
    {
        private readonly UserManager<BookUser> _userManager;
        public WishlistService(BookDbContext context, UserManager<BookUser> userManager) : base(context)
        {
            _userManager = userManager;
        }

        public async Task<WishlistDTO> AddToWishlistAsync(string curUserId, WishlistModel model)
        {
            var result = new WishlistDTO();

            // Check if the book Id is valid
            if(await _context.Books.FindAsync(model.BookId) is null)
            {
                result.Message = "Book is not Found!";
                return result;
            }
            // Check if the current user already add this book to wishlist
            //var item = await _context.Wishlists.SingleOrDefaultAsync(x => x.BookId == model.BookId && x.UserId == curUserId);
            var item = GetFirstByPredicate(x => x.BookId == model.BookId && x.UserId == curUserId);
            if (item is not null)
            {
                result.Message = "This book has been already added before!";
                return result;
            }
            // Add new Item to wishlist
            var newItem = new Wishlist()
            {
                BookId = model.BookId,
                UserId = curUserId,
                Created_at = DateTime.Now,
            };
            // Add To Database
            await _context.Wishlists.AddAsync(newItem);

            // Add To DTO
            result = await WishlistDataDTO(curUserId, model.BookId);

            return result;
        }

        public async Task<IEnumerable<WishlistDTO>> GetUserWishlistAsync(string curUserId)
        {
            var result = new List<WishlistDTO>();

            // Check if the current user has a wishlist
            //var wishlist = await _context.Wishlists.Where(x => x.UserId == curUserId).ToListAsync();
            var wishlist = GetAllByPredicate(x => x.UserId == curUserId).ToList();
            if (wishlist.Count <= 0)
                return null!;

            foreach (var item in wishlist)
                result.Add(await WishlistDataDTO(curUserId, item.BookId));

            return result;
        }

        public bool RemoveFromWishlist(string curUserId, WishlistModel model)
        {
            // Check if the requested book is not in the wishlist
            //var item = await _context.Wishlists.SingleOrDefaultAsync(x => x.UserId == curUserId && x.BookId == model.BookId);
            var item = GetFirstByPredicate(x => x.BookId == model.BookId && x.UserId == curUserId);
            if (item is null)
                return false;

            // Remove from wishlist
            _context.Wishlists.Remove(item);

            return true;
        }



        private async Task<WishlistDTO> WishlistDataDTO(string curUserId, int bookId)
        {
            var result = new WishlistDTO();

            var user = await _userManager.FindByIdAsync(curUserId);
            result.UserName = user?.UserName;

            var book = await _context.Books.Include(x => x.Reviews).SingleOrDefaultAsync(x => x.Id == bookId);

            var bookRate = 0m;
            foreach (var rate in book.Reviews)
                bookRate += rate.Rate;
            bookRate = bookRate/(book.Reviews.Count == 0? 1 : book.Reviews.Count);

            result.BookData = new WishlistBookDTO()
            {
                Id = bookId,
                Title = book.Title,
                Desscription = book.Description,
                CoverImage = book.BookCoverImage,
                Rate = bookRate
            };

            return result;
        }
    }
}
