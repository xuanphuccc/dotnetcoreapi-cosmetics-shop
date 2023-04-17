using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.ProductService;
using web_api_cosmetics_shop.Services.UserReviewService;
using web_api_cosmetics_shop.Services.UserService;

namespace web_api_cosmetics_shop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserReviewsController : ControllerBase
    {
        private readonly IUserReviewService _userReviewService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        public UserReviewsController(
            IUserReviewService userReviewService,
            IUserService userService,
            IProductService productService)
        {
            _userReviewService = userReviewService;
            _userService = userService;
            _productService = productService;
        }

        [NonAction]
        private async Task<UserReviewDTO> ConvertToUserReviewDto(UserReview userReview)
        {
            AppUser appUser = new();
            if (!string.IsNullOrEmpty(userReview.UserId))
            {
                appUser = await _userService.GetUserById(userReview.UserId);
            }

            AppUserDTO appUserDto = new();
            if (appUser != null)
            {
                appUserDto = _userService.ConvertToAppUserDto(appUser);
            }


            var userReviewDto = _userReviewService.ConvertToUserReviewDto(userReview);
            userReviewDto.User = appUserDto;

            return userReviewDto;
        }

        // Get product reviews
        [HttpGet("product/{id?}")]
        public async Task<IActionResult> GetProductReviews([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            // Get exist product
            var existProduct = await _productService.GetProductById(id.Value);
            if (existProduct == null)
            {
                return NotFound(new ErrorDTO() { Title = "product not found", Status = 404 });
            }

            // Get product reviews
            var productReviewQuery = _userReviewService.GetProductReviews(existProduct);


            var productReviews = await productReviewQuery.ToListAsync();

            // Convert to data transfer object
            List<UserReviewDTO> productReviewsDto = new();
            foreach (var review in productReviews)
            {
                productReviewsDto.Add(await ConvertToUserReviewDto(review));
            }

            return Ok(new ResponseDTO()
            {
                Data = productReviewsDto
            });
        }

        // Create review
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReview([FromBody] UserReviewDTO userReviewDto)
        {
            if (userReviewDto == null)
            {
                return BadRequest();
            }

            // Get user from token
            var userIdentity = _userService.GetCurrentUser(HttpContext.User);
            if (userIdentity == null)
            {
                return Unauthorized();
            }

            // Get user from database
            var currentUser = await _userService.GetUserByUserName(userIdentity.UserName);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            try
            {
                var newReview = new UserReview()
                {
                    RatingValue = userReviewDto.RatingValue,
                    Title = userReviewDto.Title,
                    Comment = userReviewDto.Comment,
                    CommentDate = DateTime.UtcNow,
                    UserId = currentUser.UserId,
                    OrderItemId = userReviewDto.OrderItemId,
                };

                var createdReview = await _userReviewService.AddReview(newReview);

                return Ok(new ResponseDTO()
                {
                    Data = await ConvertToUserReviewDto(createdReview)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorDTO() { Title = ex.Message, Status = 500 });
            }
        }

        // Update review
        [HttpPut("{id?}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview([FromRoute] int? id, [FromBody] UserReviewDTO userReviewDto)
        {
            if (!id.HasValue || userReviewDto == null)
            {
                return BadRequest();
            }

            var existReview = await _userReviewService.GetReviewById(id.Value);
            if (existReview == null)
            {
                return NotFound(new ErrorDTO() { Title = "review not found", Status = 404 });
            }

            // Get user from token
            var userIdentity = _userService.GetCurrentUser(HttpContext.User);
            if (userIdentity == null)
            {
                return Unauthorized();
            }

            // Get user from database
            var currentUser = await _userService.GetUserByUserName(userIdentity.UserName);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            if (existReview.UserId != currentUser.UserId)
            {
                return BadRequest(new ErrorDTO() { Title = "cannot edit other people's review" });
            }

            try
            {
                var updateReview = new UserReview()
                {
                    ReviewId = existReview.ReviewId,
                    RatingValue = userReviewDto.RatingValue,
                    Title = userReviewDto.Title,
                    Comment = userReviewDto.Comment,
                };

                if (existReview.RatingValue != updateReview.RatingValue ||
                    existReview.Title != updateReview.Title ||
                    existReview.Comment != updateReview.Comment)
                {
                    var updatedReview = await _userReviewService.UpdateReview(updateReview);

                    return Ok(new ResponseDTO()
                    {
                        Data = await ConvertToUserReviewDto(updatedReview)
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorDTO() { Title = ex.Message, Status = 500 });
            }

            return Ok(new ResponseDTO()
            {
                Data = await ConvertToUserReviewDto(existReview),
                Status = 304,
                Title = "not modified"
            });
        }

        // Delete review
        [HttpDelete("{id?}")]
        [Authorize]
        public async Task<IActionResult> RemoveReview([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            var existReview = await _userReviewService.GetReviewById(id.Value);
            if (existReview == null)
            {
                return NotFound(new ErrorDTO() { Title = "review not found", Status = 404 });
            }

            // Get user from token
            var userIdentity = _userService.GetCurrentUser(HttpContext.User);
            if (userIdentity == null)
            {
                return Unauthorized();
            }

            // Get user from database
            var currentUser = await _userService.GetUserByUserName(userIdentity.UserName);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            if (existReview.UserId != currentUser.UserId)
            {
                return BadRequest(new ErrorDTO() { Title = "cannot delete other people's review" });
            }

            try
            {
                await _userReviewService.RemoveReview(existReview);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorDTO() { Title = ex.Message, Status = 500 });
            }

            return Ok(new ResponseDTO()
            {
                Data = await ConvertToUserReviewDto(existReview)
            });
        }
    }
}
