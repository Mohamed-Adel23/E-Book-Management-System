using EBMS.Infrastructure.IServices;
using EBMS.Infrastructure.IServices.IEmail;
using EBMS.Infrastructure.IServices.IPayment;

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
        IOrderService Orders { get; }
        IPaypalService PayWithPaypal { get; }
        IEmailService SendEmail { get; }

        Task<int> CompleteAsync();
    }
}
