using EBMS.Infrastructure.IServices;

namespace EBMS.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        // Repositories
        ICategoryService Categories { get; }
        IAuthorService Authors { get; }
        IBookService Books { get; }
        IReviewService Reviews { get; }
        IWishlistService Wishlists { get; }

        Task<int> CompleteAsync();
    }
}
