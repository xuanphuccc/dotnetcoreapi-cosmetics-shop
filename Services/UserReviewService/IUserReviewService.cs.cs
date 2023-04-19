using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.UserReviewService
{
    public interface IUserReviewService
    {
        //Test
        Task<bool> IsReview(int orderItemId);
        // Get
        Task<List<UserReview>> GetAllUserReview();
        Task<UserReview> GetUserReviewByReviewId(int reviewId);
        Task<UserReview> GetUserReviewByOrderitemId(int orderItemId);
        Task<List<UserReview>> GetUserReviewByProductId(int productId);
        Task<List<UserReview>> GetUserReviewByOrderId(int orderId);

        Task<UserReviewDTO> ConvertUserReviewDTOAsync(UserReview userReview);
        // Add
        Task<UserReview> AddUserReview(UserReview userReview);
        // put
        Task<UserReview> UpdateUserReview(UserReview userReview);
        //delete
        Task<int> RemoveUserReview(UserReview userReview);
    }
}
