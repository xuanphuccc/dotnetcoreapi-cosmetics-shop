using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using web_api_cosmetics_shop.Data;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.UserReviewService
{
    public class UserReviewService : IUserReviewService
    {
        private readonly CosmeticsShopContext _context;
        public UserReviewService(CosmeticsShopContext context)
        {
            _context = context;
        }

        // Get reviews
        public async Task<List<UserReview>> GetAllReviews()
        {
            var allReviews = await _context.UserReviews.ToListAsync();

            return allReviews;
        }

        public async Task<UserReview> GetReviewById(int reviewId)
        {
            var review = await _context.UserReviews.FirstOrDefaultAsync(r => r.ReviewId == reviewId);

            return review!;
        }

        // Add review
        public async Task<UserReview> AddReview(UserReview userReview)
        {
            await _context.UserReviews.AddAsync(userReview);
            var result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                throw new Exception("cannot add a review");
            }

            return userReview;
        }

        // Update review
        public async Task<UserReview> UpdateReview(UserReview userReview)
        {
            var existReview = await GetReviewById(userReview.ReviewId);
            if (existReview == null)
            {
                throw new Exception("review not found");
            }

            existReview.RatingValue = userReview.RatingValue;
            existReview.Title = userReview.Title;
            existReview.Comment = userReview.Comment;

            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                throw new Exception("cannot update review");
            }

            return existReview;
        }

        // Remove review
        public async Task<int> RemoveReview(UserReview userReview)
        {
            _context.UserReviews.Remove(userReview);

            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                throw new Exception("cannot remove review");
            }

            return result;
        }

        // Filter
        public IQueryable<UserReview> GetProductReviews(Product product)
        {
            var productReviews = from p in _context.Products
                                 join pi in _context.ProductItems on p.ProductId equals pi.ProductId
                                 join oi in _context.OrderItems on pi.ProductItemId equals oi.ProductItemId
                                 join ur in _context.UserReviews on oi.OrderItemId equals ur.OrderItemId
                                 where p.ProductId == product.ProductId
                                 select ur;

            return productReviews;
        }


        // Convert to DTO
        public UserReviewDTO ConvertToUserReviewDto(UserReview userReview)
        {
            return new UserReviewDTO
            {
                ReviewId = userReview.ReviewId,
                RatingValue = userReview.RatingValue,
                Title = userReview.Title,
                Comment = userReview.Comment,
                CommentDate = userReview.CommentDate,
                UserId = userReview.UserId,
                OrderItemId = userReview.OrderItemId,
            };
        }
    }
}
