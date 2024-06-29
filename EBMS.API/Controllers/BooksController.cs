using EBMS.Infrastructure;
using EBMS.Infrastructure.DTOs.Book;
using Microsoft.AspNetCore.Mvc;

namespace EBMS.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public BooksController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost(Name = "createBook")]
        public async Task<IActionResult> CreateBookAsync(BookModel model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _unitOfWork.Books.CreateAsync(model);

            if(!string.IsNullOrEmpty(result.Message))
                return BadRequest(result);

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
    }
}
