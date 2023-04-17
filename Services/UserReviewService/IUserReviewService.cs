using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.UserReviewService
{
    public interface IUserReviewService
    {
        // Get reviews
        Task<List<UserReview>> GetAllReviews();
        Task<UserReview> GetReviewById(int reviewId);

        // Add review
        Task<UserReview> AddReview(UserReview userReview);

        // Update review
        Task<UserReview> UpdateReview(UserReview userReview);

        // Remove review
        Task<int> RemoveReview(UserReview userReview);

        // Filter
        IQueryable<UserReview> GetProductReviews(Product product);


        // Convert to DTO
        UserReviewDTO ConvertToUserReviewDto(UserReview userReview);
    }
}
