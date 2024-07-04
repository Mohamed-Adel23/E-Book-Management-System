using EBMS.Data.DataAccess;
using EBMS.Infrastructure.DTOs.Author;
using EBMS.Infrastructure.DTOs.Book;
using EBMS.Infrastructure.DTOs.Category;
using EBMS.Infrastructure.Helpers;
using EBMS.Infrastructure.IServices;
using EBMS.Infrastructure.Models;
using EBMS.Infrastructure.Queries;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
            fileName = $"{Guid.NewGuid().ToString().Substring(0, 15)}-EBMS{Path.GetExtension(fileName).ToLowerInvariant()}";
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
            imageName = $"{Guid.NewGuid().ToString().Substring(0, 15)}-EBMS{Path.GetExtension(imageName).ToLowerInvariant()}";
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

        public async Task<IEnumerable<BookDTO>> GetAllBooksAsync()
        {
            var result = new List<BookDTO>();
            var books = await GetAllAsync();

            foreach (var book in books)
                result.Add(await BookDataDTO(book));

            return result;
        }

        public async Task<BookDTO> UpdateAsync(int id, BookModel model)
        {
            var result = new BookDTO();

            var book = await GetByIdAsync(id);
            // Check if the Book Id is valid
            if(book is null)
            {
                result.Message = "Invalid Book Id!";
                return result;
            }

            // Check if Author Id is valid
            if (book.AuthorId != model.AuthorId && await _context.Authors.FindAsync(model.AuthorId) is null)
            {
                result.Message = "Invalid Author Id!";
                return result;
            }
            // Check if categories ids are valid
            string[] catIds = model.CategoriesIds.Split(",");
            foreach (string catId in catIds)
            {
                int cId;
                if (!int.TryParse(catId, out cId) || await _context.Categories.FindAsync(cId) is null)
                {
                    result.Message = "Invalid Category Id!";
                    return result;
                }
            }
            
            // 1- Book File
            if (model.BookFilePath is not null)
            {
                // Check The Extension 
                var fileExtension = Path.GetExtension(model.BookFilePath.FileName).ToLowerInvariant();
                if (!_allowedFileExtensions.Contains(fileExtension))
                {
                    result.Message = $"Invalid Book File Extensuion, Allowed Ones are ({string.Join(",", _allowedFileExtensions)})";
                    return result;
                }
                // Check The Size 
                if (model.BookFilePath.Length > _maxFileSize)
                {
                    result.Message = $"File Size Exceeds The Size Limit, You should upload files with size less than or equal to {_maxFileSize / (1024 * 1024 * 1024)}GB";
                    return result;
                }
                // Remove the Old File From Server
                var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Book\\Files", book.BookFilePath);
                Thread.Sleep(1000);
                File.Delete(oldFilePath);

                var fileName = model.BookFilePath.FileName;
                // Create New Fake Name for the file
                fileName = $"{Guid.NewGuid().ToString().Substring(0, 15)}-EBMS{Path.GetExtension(fileName).ToLowerInvariant()}";
                // Get The Actual Path to store on server
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Book\\Files", fileName);
                // Move The File
                using var fileStream = new FileStream(filePath, FileMode.Create);
                await model.BookFilePath.CopyToAsync(fileStream);

                // Update Book File
                book.BookFilePath = fileName;
            }
            // 2- Book Cover Image
            if (model.BookCoverImage is not null)
            {
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
                // Remove the Old Cover Image From Server
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "Book\\CoverImages", book.BookCoverImage);
                Thread.Sleep(1000);
                File.Delete(oldImagePath);

                var imageName = model.BookCoverImage.FileName;
                imageName = $"{Guid.NewGuid().ToString().Substring(0, 15)}-EBMS{Path.GetExtension(imageName).ToLowerInvariant()}";
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "Book\\CoverImages", imageName);
                // Move The Image
                using var imageStream = new FileStream(imagePath, FileMode.Create);
                await model.BookCoverImage.CopyToAsync(imageStream);

                // Update Book Cover Image
                book.BookCoverImage = imageName;
            }

            // Update Book Data
            book.Title = model.Title;
            book.Description = model.Description;
            book.PhysicalPrice = model.PhysicalPrice;
            book.DownloadPrice = model.DownloadPrice;
            book.Discount = model.Discount;
            book.AvailableQuantity = model.AvailableQuantity;
            book.Published_at = model.Published_at;
            book.AuthorId = model.AuthorId;
            book.Updated_at = DateTime.Now;

            // Update Book Data
            _context.Books.Update(book);

            // Here we should store in BookCategories
            var bookCats = new List<BookCategory>();
            // Get all categories for this book
            var cats = _context.BookCategories.Where(x => x.BookId == book.Id).ToList();
            // Delete Them
            _context.BookCategories.RemoveRange(cats);
            // Save changes
            await _context.SaveChangesAsync();
            foreach (var catId in catIds)
            {
                bookCats.Add(new BookCategory()
                {
                    BookId = book.Id,
                    CategoryId = int.Parse(catId)
                });
            }
            await _context.BookCategories.AddRangeAsync(bookCats);
            // Save Changes into Database To get required Ids
            await _context.SaveChangesAsync();

            // Store in DTO
            result = await BookDataDTO(book);

            return result;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var book = await GetByIdAsync(id);
            // Check if the Book Id is valid
            if (book is null)
                return false;

            // Remove Book Files
            var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Book\\Files", book.BookFilePath);
            Thread.Sleep(1000);
            File.Delete(oldFilePath);
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "Book\\CoverImages", book.BookCoverImage);
            Thread.Sleep(1000);
            File.Delete(oldImagePath);

            // Remove Book From Memory
            _context.Books.Remove(book);

            return true;
        }

        // Features
        public async Task<IEnumerable<BookDTO>> GetAuthorBooksAsync(int id)
        {
            var result = new List<BookDTO>();
            // Check if the Author Id is valid 
            var author = await _context.Authors.FindAsync(id);
            if (author is null)
                return null!;
            // Get All Books for this Author
            var books = _context.Books.Where(x => x.AuthorId == id);

            foreach (var book in books)
                result.Add(await BookDataDTO(book, true));

            return result;
        }

        public async Task<IEnumerable<BookDTO>> GetCategoryBooksAsync(int id)
        {
            var result = new List<BookDTO>();
            // Check if the Category Id is valid 
            var cat = await _context.Categories.FindAsync(id);
            if (cat is null)
                return null!;
            // Get All Books for this Category
            // --> This will get all the ids of related books and
            //     under the hood make join with book table to get book data 
            var bookCats = _context.BookCategories.Include(x => x.Book).Where(x => x.CategoryId == id);

            foreach (var book in bookCats)
                result.Add(await BookDataDTO(book.Book));

            return result;
        }

        public async Task<IEnumerable<BookDTO>> GetBooksByPublicationDateRangeAsync(string from, string to)
        {
            var result = new List<BookDTO>();

            // Check if the date is valid
            DateOnly fromDate;
            DateOnly toDate;
            if (!DateOnly.TryParse(from, out fromDate) || !DateOnly.TryParse(to, out toDate))
                return null!;

            if (fromDate > toDate)
                return null!;

            var books = _context.Books.Where(x => x.Published_at >= fromDate && x.Published_at <= toDate);

            if(books.Count() <= 0)
                return null!;

            foreach (var book in books)
                result.Add(await BookDataDTO(book));

            return result;
        }

        public async Task<IEnumerable<BookDTO>> GetBooksByRateAsync(decimal rate)
        {
            var result = new List<BookDTO>();

            var books = await _context.Books.Include(x => x.Reviews).ToListAsync();

            foreach(var book in books)
            {
                decimal avgRate = 0m;
                // Calculate The AVG Rate for each book
                foreach(var review in book.Reviews!)
                    avgRate += review.Rate;
                avgRate = avgRate / (book.Reviews.Count() == 0 ? 1 : book.Reviews.Count());

                if(avgRate >= rate)
                    result.Add(await BookDataDTO(book));
            }

            if (result.Count() <= 0)
                return null!;

            return result;
        }

        // Big Search
        public async Task<IEnumerable<BookDTO>> SearchAsync(string query)
        {
            var result = new List<BookDTO>();

            // HashSet to prevent book duplications
            HashSet<int> bookIds = new HashSet<int>();
            // Search By LIKE Clause
            // By Title
            var booksByTitle = _context.Books.FromSql($"SELECT * FROM Books WHERE Title LIKE '%'+{query}+'%'");
            foreach (var book in booksByTitle)
            {
                bookIds.Add(book.Id);
                result.Add(await BookDataDTO(book));
            }

            // By Description
            var booksByDesc = _context.Books.FromSql($"SELECT * FROM Books WHERE Description LIKE '%'+{query}+'%'");
            foreach (var book in booksByDesc)
            {
                if (!bookIds.Contains(book.Id))
                {
                    bookIds.Add(book.Id);
                    result.Add(await BookDataDTO(book));
                }
            }

            // By Category
            var cats = _context.Categories.FromSql($"SELECT * FROM Categories WHERE Title LIKE '%'+{query}+'%'").Include(x => x.BookCategories);
            var books = _context.Books.ToList();
            foreach (var cat in cats)
            {
                foreach (var bc in cat.BookCategories!)
                {
                    if (!bookIds.Contains(bc.BookId))
                    {
                        bookIds.Add(bc.BookId);
                        result.Add(await BookDataDTO(await _context.Books.FindAsync(bc.BookId)));
                    }
                }
            }

            // By Author
            var booksByAuthor = _context.Authors.FromSql($"SELECT * FROM Authors WHERE FullName LIKE '%'+{query}+'%'").Include(x => x.Books);
            foreach (var author in booksByAuthor)
            {
                foreach (var book in author.Books)
                {
                    if (!bookIds.Contains(book.Id))
                    {
                        bookIds.Add(book.Id);
                        result.Add(await BookDataDTO(book));
                    }
                }
            }

            if (result.Count() <= 0)
                return null!;

            return result;
        }

        // Searching, Sorting, Pagination
        public async Task<PagedList<BookDTO>> FilterBooksAsync(GetBookQueries request)
        {
            var result = new List<BookDTO>();
            IQueryable<Book> books = _context.Books;

            // Search for relevant books by Title and description
            if(request.query is not null)
                books = books.Where(x => x.Title.Contains(request.query) || x.Description!.Contains(request.query));

            // Sort By Column (Recommended to add Indexes to these columns)
            if(request.sortColumn is not null && request.sortOrder?.ToLowerInvariant() == "desc")
            {
                books = books.OrderByDescending(GetSortProperty(request.sortColumn));
            }
            else if(request.sortColumn is not null)
            {
                books = books.OrderBy(GetSortProperty(request.sortColumn));
            }

            if (books.Count() <= 0)
                return null!;

            // Apply Pagination with Skip and Take
            //books = books.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize);

            foreach (var book in books)
                result.Add(await BookDataDTO(book));

            // Wrapping The Pagination and give more information
            var pagination = PagedList<BookDTO>.Pagination(result, request.Page, request.PageSize);

            return pagination;
        }



        // Get Sort property To pass to OrderBy
        private Expression<Func<Book, Object>> GetSortProperty(string sortColumn) =>
            sortColumn.ToLowerInvariant() switch 
            {
                "title" => book => book.Title,
                "pprice" => book => book.PhysicalPrice,
                "rprice" => book => book.DownloadPrice,
                "discount" => book => book.Discount,
                "quantity" => book => book.AvailableQuantity,
                "pupdate" => book => book.Published_at,
                _ => book => book.Id
            };

        private async Task<BookDTO> BookDataDTO(Book model, bool isAuthor = false)
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

            // Get The Rate
            IQueryable<Review> bookReviews = _context.Reviews.Where(x => x.BookId == model.Id);
            decimal avgRate = 0m;
            foreach (var review in bookReviews)
                avgRate += review.Rate;

            result.Rate = avgRate/(bookReviews.Count() == 0? 1 : bookReviews.Count());

            // Select The Author 
            if (!isAuthor)
            {
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
            }
            
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
