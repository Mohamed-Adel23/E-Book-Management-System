using EBMS.Data.DataAccess;
using EBMS.Data.Services;
using EBMS.Infrastructure;
using EBMS.Infrastructure.IServices;
using EBMS.Infrastructure.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

namespace EBMS.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BookDbContext _context;
        // wwwroot Location
        private readonly IWebHostEnvironment _webHostEnvironment;
        // User Manhement
        private readonly UserManager<BookUser> _userManager;

        // Services (Repositories)
        public ICategoryService Categories { get; private set; }
        public IAuthorService Authors { get; private set; }
        public IBookService Books { get; private set; }
        public IReviewService Reviews { get; private set; }
        public IWishlistService Wishlists { get; private set; }

        // Inject The Context and Initialize The Services (Repositories)
        public UnitOfWork(BookDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<BookUser> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            Categories = new CategoryService(_context);
            Authors = new AuthorService(_context);
            Books = new BookService(_context, _webHostEnvironment);
            Reviews = new ReviewService(_context, _userManager);
            Wishlists = new WishlistService(_context, _userManager);
        }

        // Save Changes into Database with UoW, returns number of affected rows
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Damage The Current Context and Release its ralated resources
        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
