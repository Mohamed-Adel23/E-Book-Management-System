using Asp.Versioning;
using EBMS.Infrastructure;
using EBMS.Infrastructure.DTOs.Review;
using EBMS.Infrastructure.Helpers.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace EBMS.API.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    [EnableRateLimiting(RateLimiterConstants.SlidingWindow)]
    public class ReviewsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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

        [AllowAnonymous]
        [HttpGet("{id:int}", Name = "getReview")]
        public IActionResult GetReviewByIdAsync(int id)
        {
            var result = _unitOfWork.Reviews.GetReviewByIdAsync(id);

            if (!string.IsNullOrEmpty(result.Message))
                return NotFound(result);

            return Ok(result);
        }

        [Authorize(Roles = $"{RolesConstants.SuperAdmin},{RolesConstants.Admin}")]
        [HttpGet("getAll", Name = "getAllReviews")]
        public async Task<IActionResult> GetAllReviewsAsync()
        {
            var result = await _unitOfWork.Reviews.GetAllReviewsAsync();

            if (result is null)
                return NotFound("No Reviews Yet!");

            return Ok(result);
        }

        [HttpPut("{id:int}", Name = "updateReview")]
        public async Task<IActionResult> UpdateReviewAsync(int id, ReviewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Current Authenticated User
            var curUserId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = _unitOfWork.Reviews.UpdateAsync(id, curUserId!, model);

            if (!string.IsNullOrEmpty(result.Message))
                return NotFound(result);

            // Save Changes To Database
            await _unitOfWork.CompleteAsync();

            return Ok(result);
        }

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
        [AllowAnonymous]
        [HttpGet("Book/{id:int}", Name = "getBookReviews")]
        public async Task<IActionResult> GetBookReviewsAsync(int id)
        {
            var result = await _unitOfWork.Reviews.GetBookReviewsAsync(id);

            if (result is null)
                return NotFound("Book is not Found!");

            if (result.Count() <= 0)
                return NotFound("Book doesn't have any review yet!");

            return Ok(result);
        }

        // can be accessed anly by the current user, SuperAdmin and Admins
        [HttpGet("User/{userName}", Name = "getUserReviews")]
        public async Task<IActionResult> GetUserReviewsAsync(string userName)
        {
            // Current Authenticated User
            var curUserId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _unitOfWork.Reviews.GetUserReviewsAsync(userName, curUserId);

            if (result is null)
                return NotFound("Something went wrong!");

            if (result.Count() <= 0)
                return NotFound("User doesn't have any review yet!");

            return Ok(result);
        }
    }
}
