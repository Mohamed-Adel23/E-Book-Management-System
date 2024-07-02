using EBMS.Infrastructure.DTOs.Author;
using EBMS.Infrastructure.DTOs.Review;
using EBMS.Infrastructure.Models;

namespace EBMS.Infrastructure.IServices
{
    public interface IReviewService : IBaseService<Review>
    {
        Task<ReviewDTO> CreateAsync(string curUserId, ReviewModel model);
        Task<ReviewDTO> GetReviewByIdAsync(int id);
        Task<IEnumerable<ReviewDTO>> GetAllReviewsAsync();
        Task<ReviewDTO> UpdateAsync(int id, string curUserId, ReviewModel model);
        Task<bool> DeleteAsync(int id, string curUserId);
    }
}
