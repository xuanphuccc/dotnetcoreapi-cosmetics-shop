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
            var allUserReview= await _context.UserReviews.ToListAsync();
            return allUserReview;

        }

        public Task<UserReview> GetUserReview(int reviewId)
        {
            throw new NotImplementedException();
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
    }
}
