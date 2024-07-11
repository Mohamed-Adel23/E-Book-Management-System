using Asp.Versioning;
using EBMS.Infrastructure;
using EBMS.Infrastructure.DTOs.Book;
using EBMS.Infrastructure.Helpers.Constants;
using EBMS.Infrastructure.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace EBMS.API.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [EnableRateLimiting(RateLimiterConstants.TokenBucket)]
    public class BooksController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public BooksController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize(Roles = $"{RolesConstants.SuperAdmin},{RolesConstants.Admin}")]
        [HttpPost(Name = "createBook")]
        public async Task<IActionResult> CreateBookAsync(BookModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _unitOfWork.Books.CreateAsync(model);

            if (!string.IsNullOrEmpty(result.Message))
                return BadRequest(result);

            // Save Changes To Database
            await _unitOfWork.CompleteAsync();

            // Create Link of Created Book
            var url = Url.Link("getBook", new { id = result.Id });

            return Created(url, result);
        }

        [HttpGet("{id:int}", Name = "getBook")]
        public async Task<IActionResult> GetBookByIdAsync(int id)
        {
            var result = await _unitOfWork.Books.GetBookByIdAsync(id);

            if (!string.IsNullOrEmpty(result.Message))
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("getAll", Name = "getAllBooks")]
        public async Task<IActionResult> GetAllBooksAsync()
        {
            var result = await _unitOfWork.Books.GetAllBooksAsync();

            if (result is null)
                return NotFound("There are not Books Yet!");

            return Ok(result);
        }

        [Authorize(Roles = $"{RolesConstants.SuperAdmin},{RolesConstants.Admin}")]
        [HttpPut("{id:int}", Name = "updateBook")]
        public async Task<IActionResult> UpdateBookAsync(int id, BookModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _unitOfWork.Books.UpdateAsync(id, model);

            if (!string.IsNullOrEmpty(result.Message))
                return BadRequest(result);

            // Save Changes To Database
            await _unitOfWork.CompleteAsync();

            return Ok(result);
        }

        // This Action can be accessed only by Super Admin, because of its sensitivity
        [Authorize(Roles = $"{RolesConstants.SuperAdmin}")]
        [HttpDelete("{id:int}", Name = "deleteBook")]
        public async Task<IActionResult> DeleteBookAsync(int id)
        {
            var result = await _unitOfWork.Books.DeleteAsync(id);

            if (!result)
                return NotFound("Invalid Book Id!");

            // Save Changes To Database
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [Authorize]
        [HttpGet("download/{id:int}", Name = "downloadBook")]
        public async Task<IActionResult> DownloadAsync(int id)
        {
            // Current Authenticated User
            var curUserId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _unitOfWork.Books.DownloadAsync(curUserId!, id);

            if (!string.IsNullOrEmpty(result.Message))
                return BadRequest(new { result.Message });

            // Save Changes To Database
            await _unitOfWork.CompleteAsync();

            return File(result.MemoryDataStream, result.ContentType, result.FileName);
        }

        // Features
        [HttpGet("Author/{id:int}", Name = "getAuthorBooks")]
        public async Task<IActionResult> GetAuthorBooksAsync(int id)
        {
            var result = await _unitOfWork.Books.GetAuthorBooksAsync(id);

            if (result is null)
                return NotFound("Author is not found!");

            if (result.Count() <= 0)
                return NotFound("Author doesn't have any book yet!");

            return Ok(result);
        }

        [HttpGet("Category/{id:int}", Name = "getCategoryBooks")]
        public async Task<IActionResult> GetCategoryBooksAsync(int id)
        {
            var result = await _unitOfWork.Books.GetCategoryBooksAsync(id);

            if (result is null)
                return NotFound("category is not found!");

            if (result.Count() <= 0)
                return NotFound("Category doesn't have any book yet!");

            return Ok(result);
        }

        [HttpGet("Date", Name = "getBooksByPublicationDate")]
        public async Task<IActionResult> GetBooksByPublicationDateRangeAsync([FromQuery] string from, [FromQuery] string to)
        {
            var result = await _unitOfWork.Books.GetBooksByPublicationDateRangeAsync(from, to);

            if (result is null)
                return BadRequest("Something went wrong!");

            return Ok(result);
        }

        [HttpGet("Rate", Name = "getBooksByRate")]
        public async Task<IActionResult> GetBooksByRateAsync([FromQuery] decimal minrate)
        {
            var result = await _unitOfWork.Books.GetBooksByRateAsync(minrate);

            if (result is null)
                return BadRequest("Somthing went wrong!");

            return Ok(result);
        }

        [HttpGet("Search", Name = "search")]
        public async Task<IActionResult> SearchAsync([FromQuery] string query)
        {
            var result = await _unitOfWork.Books.SearchAsync(query);

            if (result is null)
                return BadRequest("Something went wrong!");

            return Ok(result);
        }

        // Searching, Sorting, Pagination
        [HttpGet(Name = "filerBooks")]
        public async Task<IActionResult> FilterBooksAsync(
            string? searchTerm,
            string? sortColumn,
            string? sortOrder,
            int page,
            int pageSize
            )
        {
            var query = new GetBookQueries(searchTerm, sortColumn, sortOrder, page, pageSize);
            var result = await _unitOfWork.Books.FilterBooksAsync(query);

            if (result is null)
                return BadRequest("There is no Books!");

            return Ok(result);
        }
    }
}
