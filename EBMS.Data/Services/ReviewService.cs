using EBMS.Data.DataAccess;
using EBMS.Infrastructure.DTOs.Review;
using EBMS.Infrastructure.Helpers;
using EBMS.Infrastructure.IServices;
using EBMS.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EBMS.Data.Services
{
    public class ReviewService : BaseService<Review>, IReviewService
    {
        private readonly UserManager<BookUser> _userManager;
        public ReviewService(BookDbContext context, UserManager<BookUser> userManager) : base(context)
        {
            _userManager = userManager;
        }

        public async Task<ReviewDTO> CreateAsync(string curUserId, ReviewModel model)
        {
            ReviewDTO result = new();
            // Check if the userId is valid
            var user = await _userManager.FindByIdAsync(curUserId);
            if(user is null)
            {
                result.Message = "User is not Found!";
                return result;
            }
            // Check if BookId is valid
            var book = await _context.Books.FindAsync(model.BookId);
            if(book is null)
            {
                result.Message = "Book is not Found!";
                return result;
            }
            // Check if the current user has a review for this book
            if(await _context.Reviews.AnyAsync(x => x.UserId == curUserId && x.BookId == model.BookId))
            {
                result.Message = "You have already a review for this book!";
                return result;
            }
            // Add New Review
            var review = new Review()
            {
                Rate = model.Rate,
                Comment = model.Comment,
                UserId = curUserId,
                BookId = model.BookId,
                Created_at = DateTime.Now
            };
            // Add new Review to Database
            await _context.Reviews.AddAsync(review);
            // Save Changes to get review id
            await _context.SaveChangesAsync();

            // Store Data into DTO
            result = ReviewDataDTO(review, user, book);

            return result;
        }

        public async Task<ReviewDTO> GetReviewByIdAsync(int id)
        {
            var result = new ReviewDTO();
            // Check id the review id is valid
            var review = await _context.Reviews.Include(u => u.BookUser).Include(b => b.Book).SingleOrDefaultAsync(x => x.Id == id);
            if(review is null)
            {
                result.Message = "Review is not Found!";
                return result;
            }

            result = ReviewDataDTO(review, review.BookUser, review.Book);

            return result;
        }

        public async Task<IEnumerable<ReviewDTO>> GetAllReviewsAsync()
        {
            var result = new List<ReviewDTO>();

            var reviews = await _context.Reviews.Include(u => u.BookUser).Include(b => b.Book).ToListAsync();

            foreach (var review in reviews)
                result.Add(ReviewDataDTO(review, review.BookUser, review.Book));

            return result;
        }

        public async Task<ReviewDTO> UpdateAsync(int id, string curUserId, ReviewModel model)
        {
            var result = new ReviewDTO();
            // Check if review id is valid
            var review = await _context.Reviews.Include(u => u.BookUser).Include(b => b.Book).SingleOrDefaultAsync(x => x.Id == id);
            if (review is null)
            {
                result.Message = "Review is not Found!";
                return result;
            }
            // Check if the current user owns this review
            if (curUserId != review.UserId)
            {
                result.Message = "You can't update this review!";
                return result;
            }
            // Update Review Data
            review.Rate = model.Rate;
            review.Comment = model.Comment;
            review.Updated_at = DateTime.Now;

            // Update Data in Memory
            _context.Reviews.Update(review);

            // Store Data to DTO
            result = ReviewDataDTO(review, review.BookUser, review.Book);

            return result;
        }

        public async Task<bool> DeleteAsync(int id, string curUserId)
        {
            // Check if the id is valid
            var review = await GetByIdAsync(id);
            if (review is null)
                return false;

            // Super Admin and Admins can remove any Review
            var user = await _userManager.FindByIdAsync(curUserId);
            if(await _userManager.IsInRoleAsync(user, RolesConstants.SuperAdmin) || await _userManager.IsInRoleAsync(user, RolesConstants.Admin))
            {
                // Remove Review From Memory
                _context.Reviews.Remove(review);

                return true;
            }
            // Role Now is: Reader, So the user who owns this review can remove it
            if (curUserId != review.UserId)
                return false;

            // Remove Review From Memory
            _context.Reviews.Remove(review);

            return true;
        }



        private ReviewDTO ReviewDataDTO(Review model, BookUser user, Book book)
        {
            var result = new ReviewDTO();

            result.Id = model.Id;
            result.Rate = model.Rate;
            result.Comment = model.Comment;
            result.Created_at = model.Created_at;
            result.Updated_at = model.Updated_at;

            // Get the user
            result.User = new ReviewUserDTO()
            {
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email
            };
            // Get The Book
            result.Book = new ReviewBookDTO()
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Published_at = book.Published_at,
            };

            return result;
        }
    }
}
