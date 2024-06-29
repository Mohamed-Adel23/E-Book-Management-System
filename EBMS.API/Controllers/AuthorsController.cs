using EBMS.Infrastructure;
using EBMS.Infrastructure.DTOs.Author;
using EBMS.Infrastructure.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EBMS.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{RolesConstants.SuperAdmin},{RolesConstants.Admin}")]
    public class AuthorsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost(Name = "createAuthor")]
        public async Task<IActionResult> CreateAuthorAsync(AuthorModel model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _unitOfWork.Authors.CreateAsync(model);

            if(!string.IsNullOrEmpty(result.Message))
                return BadRequest(result);

            // Save Changes To Database
            await _unitOfWork.CompleteAsync();
            // Generate The Url of The New Author
            string url = Url.Link("getAuthor", new { id = result.Id });

            return Created(url, result);
        }

        [HttpGet("{id:int}", Name = "getAuthor")]
        public async Task<IActionResult> GetAuthorByIdAsync(int id)
        {
            var result = await _unitOfWork.Authors.GetAuthorByIdAsync(id);

            if(!string.IsNullOrEmpty(result.Message))
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("getAll", Name = "getAllAuthors")]
        public async Task<IActionResult> GetAllAuthorsAsync()
        {
            var result = await _unitOfWork.Authors.GetAllAuthorsAsync();

            if(result is null)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("{id:int}", Name = "updateAuthor")]
        public async Task<IActionResult> UpdateAuthorAsync(int id, AuthorModel model)
        {
            var result = await _unitOfWork.Authors.UpdateAsync(id, model);

            if(!string.IsNullOrEmpty(result.Message))
                return BadRequest(result);

            // save Changes To Database
            await _unitOfWork.CompleteAsync();

            return Ok(result);
        }

        [HttpDelete("{id:int}", Name = "deleteAuthor")]
        public async Task<IActionResult> DeleteAuthorAsync(int id)
        {
            var result = await _unitOfWork.Authors.DeleteAsync(id);

            if (!result)
                return NotFound("Author is not Found!");

            // save Changes To Database
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}
