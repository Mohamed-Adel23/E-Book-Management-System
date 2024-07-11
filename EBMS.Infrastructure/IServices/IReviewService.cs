using EBMS.Infrastructure.DTOs.Author;
using EBMS.Infrastructure.DTOs.Review;
using EBMS.Infrastructure.Models;

namespace EBMS.Infrastructure.IServices
{
    public interface IReviewService : IBaseService<Review>
    {
        Task<ReviewDTO> CreateAsync(string curUserId, ReviewModel model);
        ReviewDTO GetReviewByIdAsync(int id);
        Task<IEnumerable<ReviewDTO>> GetAllReviewsAsync();
        ReviewDTO UpdateAsync(int id, string curUserId, ReviewModel model);
        Task<bool> DeleteAsync(int id, string curUserId);

        // Features
        Task<IEnumerable<ReviewDTO>> GetBookReviewsAsync(int id);
        Task<IEnumerable<ReviewDTO>> GetUserReviewsAsync(string userName, string curUserId);
    }
}
