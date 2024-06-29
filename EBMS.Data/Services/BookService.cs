using EBMS.Data.DataAccess;
using EBMS.Infrastructure.DTOs.Author;
using EBMS.Infrastructure.DTOs.Book;
using EBMS.Infrastructure.DTOs.Category;
using EBMS.Infrastructure.IServices.IAuth;
using EBMS.Infrastructure.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace EBMS.Data.Services
{
    public class BookService : BaseService<Book>, IBookService
    {
        private string[] _allowedFileExtensions =  { ".pdf",".txt",".epub",".doc",".docs" };
        private int _maxFileSize = 1 * 1024 * 1024 * 1024; // 1 GB
        private string[] _allowedImageExtensions = { ".jpg", ".png", ".jpeg" };
        private int _maxImageSize = 3 * 1024 * 1024; // 3 MB

        private readonly IWebHostEnvironment _webHostEnvironment;

        public BookService(BookDbContext context, IWebHostEnvironment webHostEnvironment) : base(context)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<BookDTO> CreateAsync(BookModel model)
        {
            var result = new BookDTO();

            // Check if Author Id is valid
            if(await _context.Authors.FindAsync(model.AuthorId) is null)
            {
                result.Message = "Invalid Author Id!";
                return result;
            }
            // Check if categories ids are valid
            string[] catIds = model.CategoriesIds.Split(",");
            foreach(string catId in catIds)
            {
                int id;
                if(!int.TryParse(catId, out id) || await _context.Categories.FindAsync(id) is null)
                {
                    result.Message = "Invalid Category Id!";
                    return result;
                }
            }
            // Upload Files
            if(model.BookFilePath is null || model.BookCoverImage is null)
            {
                result.Message = "Book File and Its Cover Image are required!";
                return result;
            }
            // 1- Book File
            // Check The Extension 
            var fileExtension = Path.GetExtension(model.BookFilePath.FileName).ToLowerInvariant();
            if (!_allowedFileExtensions.Contains(fileExtension))
            {
                result.Message = $"Invalid Book File Extensuion, Allowed Ones are ({string.Join(",", _allowedFileExtensions)})";
                return result;
            }
            // Check The Size 
            if(model.BookFilePath.Length > _maxFileSize)
            {
                result.Message = $"File Size Exceeds The Size Limit, You should upload files with size less than or equal to {_maxFileSize / (1024 * 1024 * 1024)}GB";
                return result;
            }
            var fileName = model.BookFilePath.FileName;
            // Create New Fake Name for the file
            fileName = $"{Guid.NewGuid().ToString().Substring(0, 6)}-{Path.GetFileName(fileName)}";
            // Get The Actual Path to store on server
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Book\\Files", fileName);
            // Move The File
            using var fileStream = new FileStream(filePath, FileMode.Create);
            await model.BookFilePath.CopyToAsync(fileStream);

            // 2- Book Cover Image
            var imageExtension = Path.GetExtension(model.BookCoverImage.FileName).ToLowerInvariant();
            if (!_allowedImageExtensions.Contains(imageExtension))
            {
                result.Message = $"Invalid Book Cover Image Extensuion, Allowed Ones are ({string.Join(",", _allowedImageExtensions)})";
                return result;
            }
            // Check The Size 
            if (model.BookCoverImage.Length > _maxImageSize)
            {
                result.Message = $"Cover Image Size Exceeds The Size Limit, You should upload files with size less than or equal to {_maxImageSize / (1024 * 1024)}MB";
                return result;
            }
            var imageName = model.BookCoverImage.FileName;
            imageName = $"{Guid.NewGuid().ToString().Substring(0, 6)}-{Path.GetFileName(imageName)}";
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "Book\\CoverImages", imageName);
            // Move The Image
            using var imageStream = new FileStream(imagePath, FileMode.Create);
            await model.BookCoverImage.CopyToAsync(imageStream);

            // Create New Book
            var newBook = new Book()
            {
                Title = model.Title,
                Description = model.Description,
                PhysicalPrice = model.PhysicalPrice,
                DownloadPrice = model.DownloadPrice,
                Discount = model.Discount,
                AvailableQuantity = model.AvailableQuantity,
                Published_at = model.Published_at,
                BookFilePath = fileName,
                BookCoverImage = imageName,
                Created_at = DateTime.Now,
                AuthorId = model.AuthorId
            };
            // Create New Book in The Database
            await _context.Books.AddAsync(newBook);
            // Save Changes to Get the Book Id
            await _context.SaveChangesAsync();

            // Here we should store in BookCategories
            var bookCats = new List<BookCategory>();
            foreach (var catId in catIds)
            {
                bookCats.Add(new BookCategory()
                {
                    BookId = newBook.Id,
                    CategoryId = int.Parse(catId)
                });
            }
            // Store Data to database
            await _context.BookCategories.AddRangeAsync(bookCats);
            // Save Changes into Database To get required Ids
            await _context.SaveChangesAsync();

            // Store in DTO
            result = await BookDataDTO(newBook);

            return result;
        }

        public async Task<BookDTO> GetBookByIdAsync(int id)
        {
            var result = new BookDTO();
            var book = await GetByIdAsync(id);
            // Check if the id is valid 
            if (book is null)
            {
                result.Message = "Book is not Found!";
                return result;
            }
            // Store Data in DTO
            result = await BookDataDTO(book);

            return result;
        }



        private async Task<BookDTO> BookDataDTO(Book model)
        {
            var result = new BookDTO();

            result.Id = model.Id;
            result.Title = model.Title;
            result.Description = model.Description;
            result.PhysicalPrice = model.PhysicalPrice;
            result.DownloadPrice = model.DownloadPrice;
            result.Discount = model.Discount;
            result.AvailableQuantity = model.AvailableQuantity;
            result.Published_at = model.Published_at;
            result.BookFilePath = model.BookFilePath;
            result.BookCoverImage = model.BookCoverImage;
            result.Created_at = model.Created_at;
            result.Updated_at = model.Updated_at;

            // Select The Author 
            var author = await _context.Authors.FindAsync(model.AuthorId);
            result.Author = new AuthorDTO()
            {
                Id = author!.Id,
                FullName = author?.FullName,
                Bio = author?.Bio,
                ProfilePic = author?.ProfilePic,
                Created_at = author!.Created_at,
                Updated_at = author?.Updated_at,
            };

            // Select Categories
            var cats = await _context.Categories.ToListAsync();
            var catsBooks = _context.BookCategories.Where(x => x.BookId == model.Id).ToList();
            // Join Catgories and BookCategories
            result.Categories = catsBooks.Join(cats,
                    cb => cb.CategoryId,
                    c => c.Id,
                    (cb, c) => new CategoryDTO()
                    {
                        Id = c.Id,
                        Title = c.Title,
                        Description = c.Description,
                        Created_at = c.Created_at,
                        Updated_at = c.Updated_at
                    }
            ).ToList();

            return result;
        }
    }
}
