using Microsoft.AspNetCore.Mvc;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.ShopOrderService;
using web_api_cosmetics_shop.Services.UserReviewService;
using web_api_cosmetics_shop.Services.UserService;

namespace web_api_cosmetics_shop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserReviewsController : ControllerBase
    {
        private readonly IUserService _userService;
        public readonly IUserReviewService _userReviewService;
        public readonly IShopOrderService _shopOrderService;
        public UserReviewsController(IUserService userService, IUserReviewService userReviewService, IShopOrderService shopOrderService)
        {
            _userReviewService = userReviewService;
            _userService = userService;
            _shopOrderService= shopOrderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserReview()
        {
            var userReviews = await _userReviewService.GetAllUserReview();
            return Ok(new ResponseDTO()
            {
                Data = userReviews
            });
        }
        [HttpPost]
        public async Task<IActionResult> AddUserReview(UserReviewDTO userReviewDto)
        {
            if (userReviewDto == null || userReviewDto.RatingValue == 0)
            {
                return BadRequest();
            }
            // Logged user
            var currentIdentityUser = _userService.GetCurrentUser(HttpContext.User);
            if (currentIdentityUser == null)
            {
                return NotFound();
            }

            // Exist user from database
            var currentUser = await _userService.GetUserByUserName(currentIdentityUser.UserName);
            if (currentUser == null)
            {
                return NotFound();
            }
            //check order_item
            var orderItem = await _shopOrderService.GetOrderItem(userReviewDto.OrderItemId);
            if(orderItem == null)
            {
                return NotFound(new ErrorDTO() { Title = "Order Item not found", Status = 400 });
            }


            return Ok(new ResponseDTO()
            {
                Data = userReviewDto
            });
        }
    }
}
