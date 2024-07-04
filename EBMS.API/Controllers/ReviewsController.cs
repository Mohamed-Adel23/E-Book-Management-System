using EBMS.Infrastructure;
using EBMS.Infrastructure.DTOs.Review;
using EBMS.Infrastructure.Helpers.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EBMS.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpPost(Name = "createReview")]
        public async Task<IActionResult> CreateReviewAsync(ReviewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            // Current Authenticated User
            var curUserId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _unitOfWork.Reviews.CreateAsync(curUserId!, model);

            if (!string.IsNullOrEmpty(result.Message))
                return BadRequest(result);

            // Save Changes To Database
            await _unitOfWork.CompleteAsync();

            // Create a Link for the new review
            var url = Url.Link("getReview", new { id = result.Id });

            return Created(url, result);
        }

        [HttpGet("{id:int}", Name = "getReview")]
        public async Task<IActionResult> GetReviewAsync(int id)
        {
            var result = await _unitOfWork.Reviews.GetReviewByIdAsync(id);

            if (!string.IsNullOrEmpty(result.Message))
                return NotFound(result);

            return Ok(result);
        }

        [Authorize(Roles = $"{RolesConstants.SuperAdmin},{RolesConstants.Admin}")]
        [HttpGet("getAll", Name = "getAllReviews")]
        public async Task<IActionResult> GetAllReviewsAsync()
        {
            var result = await _unitOfWork.Reviews.GetAllReviewsAsync();

            if(result is null)
                return NotFound("No Reviews Yet!");

            return Ok(result);
        }

        [Authorize]
        [HttpPut("{id:int}", Name = "updateReview")]
        public async Task<IActionResult> UpdateReviewAsync(int id, ReviewModel model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            // Current Authenticated User
            var curUserId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _unitOfWork.Reviews.UpdateAsync(id, curUserId!, model);

            if(!string.IsNullOrEmpty(result.Message))
                return NotFound(result);

            // Save Changes To Database
            await _unitOfWork.CompleteAsync();

            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{id:int}", Name = "deleteReview")]
        public async Task<IActionResult> DeleteReviewAsync(int id)
        {
            // Current Authenticated User
            var curUserId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _unitOfWork.Reviews.DeleteAsync(id, curUserId!);

            if (!result)
                return BadRequest("Something went wrong");

            // Save Changes To Database
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        // Features
        [HttpGet("Book/{id:int}", Name = "getBookReviews")]
        public async Task<IActionResult> GetBookReviewsAsync(int id)
        {
            var result = await _unitOfWork.Reviews.GetBookReviewsAsync(id);

            if (result is null)
                return NotFound("Book is not Found!");

            if(result.Count() <= 0)
                return NotFound("Book doesn't have any review yet!");

            return Ok(result);
        }

        [Authorize]
        [HttpGet("User/{userName}", Name = "getUserReviews")]
        public async Task<IActionResult> GetUserReviewsAsync(string userName)
        {
            var result = await _unitOfWork.Reviews.GetUserReviewsAsync(userName);

            if (result is null)
                return NotFound("User is not Found!");

            if (result.Count() <= 0)
                return NotFound("User doesn't have any review yet!");

            return Ok(result);
        }
    }
}
