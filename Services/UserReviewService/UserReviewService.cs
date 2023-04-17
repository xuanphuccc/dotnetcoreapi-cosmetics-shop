using Microsoft.EntityFrameworkCore;
using web_api_cosmetics_shop.Data;
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
        //get

        public async Task<List<UserReview>> GetAllUserReview()
        {
            var allUserReview = await _context.UserReviews.ToListAsync();
            return allUserReview;

        }

        public async Task<UserReview> GetUserReviewByReviewId(int reviewId)
        {
            var userReview = await _context.UserReviews.FirstOrDefaultAsync(ur => ur.ReviewId == reviewId);
            return userReview;
        }

        public async Task<UserReview> GetUserReviewByOrderitemId(int orderItemId)
        {
            var userReview = await _context.UserReviews.FirstOrDefaultAsync(ur => ur.OrderItemId == orderItemId);
            return userReview;
        }

        public async Task<List<UserReview>> GetUserReviewByProductId(int productId)
        {
            var userReview = await (from p in _context.Products
                                    join pi in _context.ProductItems on p.ProductId equals pi.ProductId
                                    join oi in _context.OrderItems on pi.ProductItemId equals oi.ProductItemId
                                    join ur in _context.UserReviews on oi.OrderItemId equals ur.OrderItemId
                                    where pi.ProductId == productId
                                    select ur).ToListAsync();

            return userReview;
        }

      
        public async Task<List<UserReview>> GetUserReviewByOrderId(int orderId)
        {
            var userReview = await (from so in _context.ShopOrders
                                    join oi in _context.OrderItems on so.OrderId equals oi.OrderId
                                    join ur in _context.UserReviews on oi.OrderItemId equals ur.OrderItemId
                                    where so.OrderId == orderId
                                    select ur
                                    ).ToListAsync();
            return userReview;
        }
        public Task<List<UserReview>> GetUserReviews(string userId)
        {
            throw new NotImplementedException();
        }
        //add
        public async Task<UserReview> AddUserReview(UserReview userReview)
        {
            await _context.UserReviews.AddAsync(userReview);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return null!;
            }

            return userReview;
        }

        //delete
        public async Task<int> RemoveUserReview(UserReview userReview)
        {
            _context.Remove(userReview);
            var result = await _context.SaveChangesAsync();
            return result;
        }

        //check

        public async Task<bool> IsReview(int orderItemId)
        {
            var userReview = await GetUserReviewByOrderitemId(orderItemId);
            if (userReview == null)
            {
                return false;
            }
            return true;
        }
    }
}
