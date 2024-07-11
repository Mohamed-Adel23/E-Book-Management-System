using EBMS.Data.DataAccess;
using EBMS.Data.Services;
using EBMS.Data.Services.Email;
using EBMS.Data.Services.Payment;
using EBMS.Infrastructure;
using EBMS.Infrastructure.IServices;
using EBMS.Infrastructure.IServices.ICache;
using EBMS.Infrastructure.IServices.IEmail;
using EBMS.Infrastructure.IServices.IPayment;
using EBMS.Infrastructure.IServices.IFile;
using EBMS.Infrastructure.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace EBMS.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BookDbContext _context;
        // wwwroot Location
        private readonly IWebHostEnvironment _webHostEnvironment;
        // User Management
        private readonly UserManager<BookUser> _userManager;
        //Payment Properties
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _configuration;
        // cache Srevice
        private readonly ICacheService _cacheService;
        // Email Service
        private readonly IEmailService _emailService;
        // Upload File Service
        private readonly IFileService _fileService;

        // Services (Repositories)
        public ICategoryService Categories { get; private set; }
        public IAuthorService Authors { get; private set; }
        public IBookService Books { get; private set; }
        public IReviewService Reviews { get; private set; }
        public IWishlistService Wishlists { get; private set; }
        public IOrderService Orders { get; private set; }
        public IPaypalService PayWithPaypal { get; private set; }
        public IEmailService SendEmail { get; private set; }

        // Inject The Context and Initialize The Services (Repositories)
        public UnitOfWork(BookDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<BookUser> userManager, IHttpContextAccessor contextAccessor, IConfiguration configuration, ICacheService cacheService, IEmailService emailService, IFileService fileService)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _configuration = configuration;
            _cacheService = cacheService;
            _emailService = emailService;
            _fileService = fileService;

            Categories = new CategoryService(_context, _cacheService);
            Authors = new AuthorService(_context, _cacheService);
            Books = new BookService(_context, _userManager, _cacheService, _fileService);
            Reviews = new ReviewService(_context, _userManager);
            Wishlists = new WishlistService(_context, _userManager);
            Orders = new OrderService(_context, _userManager);
            PayWithPaypal = new PaypalService(_context, _contextAccessor, _configuration);
            SendEmail = new EmailService(_configuration, _context, _userManager);
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
