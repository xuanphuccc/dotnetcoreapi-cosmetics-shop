using Microsoft.AspNetCore.Mvc;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.ProductService;
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
        public readonly IProductService _productService;
        public UserReviewsController(IUserService userService, IUserReviewService userReviewService,
            IShopOrderService shopOrderService, IProductService productService)
        {
            _userReviewService = userReviewService;
            _userService = userService;
            _shopOrderService = shopOrderService;
            _productService = productService;
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
        [HttpGet("product/{id?}")]
        public async Task<IActionResult> GetProductReview([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }
            var productReview = await _productService.GetProductById(id.Value);
            if (productReview == null)
            {
                return NotFound(new ErrorDTO() { Title = "product not found", Status = 404 });
            }
            var userReviewsProduct = await _userReviewService.GetUserReviewByProductId(id.Value);
            var userReviewsProductDto = new List<UserReviewDTO>();
            foreach (var userReview in userReviewsProduct)
            {
                var addUserReview = await _userReviewService.ConvertUserReviewDTOAsync(userReview);
                userReviewsProductDto.Add(addUserReview);
            }
            return Ok(new ResponseDTO()
            {
                Data = userReviewsProductDto
            });
        }
        [HttpGet("myorder/{id?}")]
        public async Task<IActionResult> GetMyOrderReview([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }
            var shoporder = await _shopOrderService.GetShopOrder(id.Value);
            if (shoporder == null)
            {
                return NotFound(new ErrorDTO() { Title = "shoporder not found", Status = 404 });
            }
            var userReviewsProduct = await _userReviewService.GetUserReviewByOrderId(id.Value);
            var userReviewsProductDto = new List<UserReviewDTO>();
            foreach (var userReview in userReviewsProduct)
            {
                var addUserReview = await _userReviewService.ConvertUserReviewDTOAsync(userReview);
                userReviewsProductDto.Add(addUserReview);
            }
            return Ok(new ResponseDTO()
            {
                Data = userReviewsProductDto
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddUserReview([FromBody] UserReviewDTO userReviewDto)
        {
            if (userReviewDto == null || userReviewDto.RatingValue == 0)
            {
                return BadRequest();
            }
            // Logged user
            var currentIdentityUser = _userService.GetCurrentUser(HttpContext.User);
            if (currentIdentityUser == null)
            {
                return NotFound(new ErrorDTO() { Title = "user not found", Status = 404 });
            }

            // Exist user from database
            var currentUser = await _userService.GetUserByUserName(currentIdentityUser.UserName);
            if (currentUser == null)
            {
                return NotFound(new ErrorDTO() { Title = "user not found", Status = 404 });
            }

            //check user rated product
            UserReview userRatedItem = new();

            //check order_item

            OrderItem orderItem = new();

            if (userReviewDto.OrderItemId.HasValue)
            {
                orderItem = await _shopOrderService.GetOrderItem(userReviewDto.OrderItemId.Value);
                userRatedItem = await _userReviewService.GetUserReviewByOrderitemId(userReviewDto.OrderItemId.Value);
            }
            if (userRatedItem != null)
            {
                return BadRequest();
            }

            if (orderItem == null)
            {
                return NotFound(new ErrorDTO() { Title = "Order Item not found", Status = 400 });
            }

            // add userReview
            try
            {
                var newUserReview = new UserReview()
                {
                    UserId = currentUser.UserId,
                    RatingValue = userReviewDto.RatingValue,
                    Title = userReviewDto.Title,
                    Comment = userReviewDto.Comment,
                    CommentDate = DateTime.UtcNow,
                    OrderItemId = userReviewDto.OrderItemId

                };
                var createdUserReview = await _userReviewService.AddUserReview(newUserReview);
                if (createdUserReview == null)
                {
                    return StatusCode(
                                StatusCodes.Status500InternalServerError,
                                new ErrorDTO() { Title = "can not create userReview", Status = 500 });
                }
                var CreateSuccess = await _userReviewService.ConvertUserReviewDTOAsync(createdUserReview);
                return Ok(new ResponseDTO()
                {
                    Data = CreateSuccess
                });

            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }
        }
        //update
        [HttpPut("{id?}")]
        public async Task<IActionResult> UpdateUserReview([FromRoute] int? id, [FromBody] UserReviewDTO userReviewDto)
        {
            if (!id.HasValue || userReviewDto == null)
            {
                return BadRequest();
            }
            try
            {
                var exitsUserReview = await _userReviewService.GetUserReviewByReviewId(id.Value);
                if (exitsUserReview == null)
                {
                    return NotFound(new ErrorDTO() { Title = "reviewId not found", Status = 400 });
                }
                var updateUserReivew = new UserReview()
                {
                    ReviewId = exitsUserReview.ReviewId,
                    RatingValue = userReviewDto.RatingValue,
                    Comment = userReviewDto.Comment,
                    Title = userReviewDto.Title,
                    OrderItemId = userReviewDto.OrderItemId
                };
                if (updateUserReivew.RatingValue != exitsUserReview.RatingValue
                    || updateUserReivew.Comment != exitsUserReview.Comment ||
                    updateUserReivew.Title != exitsUserReview.Title)
                {
                    var updateResult = await _userReviewService.UpdateUserReview(updateUserReivew);
                    if (updateResult == null)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError,
                                        new ErrorDTO() { Title = "Can not update review", Status = 500 });
                    }
                }
                var updateSuccess = await _userReviewService.ConvertUserReviewDTOAsync(updateUserReivew);
                return Ok(new ResponseDTO()
                {
                    Data = updateSuccess
                });
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

        }

        //delete 
        [HttpDelete("{id?}")]
        public async Task<IActionResult> DeleteUserReview([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }
            try
            {
                var userReview = await _userReviewService.GetUserReviewByReviewId(id.Value);
                if (userReview == null)
                {
                    return NotFound(new ErrorDTO() { Title = "user review not found", Status = 400 });
                }
                await _userReviewService.RemoveUserReview(userReview);
                var deleteSuccess = await _userReviewService.ConvertUserReviewDTOAsync(userReview);
                return Ok(
                new ResponseDTO()
                {
                    Data = deleteSuccess
                }
                );
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });

            }

        }
    }
}
