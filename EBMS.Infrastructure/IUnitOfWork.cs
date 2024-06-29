using EBMS.Infrastructure.IServices;
using EBMS.Infrastructure.IServices.IAuth;

namespace EBMS.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        // Repositories
        ICategoryService Categories { get; }
        IAuthorService Authors { get; }
        IBookService Books { get; }

        Task<int> CompleteAsync();
    }
}
