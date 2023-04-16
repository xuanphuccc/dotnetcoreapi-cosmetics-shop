using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.UserReviewService
{
    public interface IUserReviewService
    {
        // Get
        Task<List<UserReview>> GetAllUserReview();
        Task<List<UserReview>> GetUserReviews(string userId);
        Task<UserReview> GetUserReview(int reviewId);

        // Add

        Task<UserReview> AddUserReview(UserReview userReview);
    }
}
