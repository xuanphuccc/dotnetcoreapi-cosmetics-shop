﻿using Microsoft.EntityFrameworkCore;
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
        //get

        public async Task<List<UserReview>> GetAllUserReview()
        {
            var allUserReview = await _context.UserReviews.ToListAsync();
            return allUserReview;

        }

        public async Task<UserReview> GetUserReviewByReviewId(int reviewId)
        {
            var userReview = await _context.UserReviews.FirstOrDefaultAsync(ur => ur.ReviewId == reviewId);
            return userReview!;
        }

        public async Task<UserReview> GetUserReviewByOrderitemId(int orderItemId)
        {
            var userReview = await _context.UserReviews.FirstOrDefaultAsync(ur => ur.OrderItemId == orderItemId);
            return userReview!;
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
        //update

        public async Task<UserReview> UpdateUserReview(UserReview userReview)
        {
            UserReview exitsUserReview = new();
            exitsUserReview = await GetUserReviewByReviewId(userReview.ReviewId);
            if (exitsUserReview == null)
            {
                return null!;
            }
            exitsUserReview.Comment = userReview.Comment;
            exitsUserReview.RatingValue = userReview.RatingValue;
            exitsUserReview.Title = userReview.Title;
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

            if (result == 0)
            {
                throw new Exception("cannot remove review");
            }
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

        //convert DTO
        public async Task<UserReviewDTO> ConvertUserReviewDTOAsync(UserReview userReview)
        {
            var userReviewDto = new UserReviewDTO();
            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.UserId == userReview.UserId);

            if (user == null)
            {
                return new UserReviewDTO()
                {
                    RatingValue = userReview.RatingValue,
                    Title = userReview.Title,
                    Comment = userReview.Comment,
                    CommentDate = DateTime.UtcNow,
                    OrderItemId = userReview.OrderItemId,
                    ReviewId = userReview.ReviewId
                };
            }
            return new UserReviewDTO()
            {
                RatingValue = userReview.RatingValue,
                Title = userReview.Title,
                Comment = userReview.Comment,
                CommentDate = DateTime.UtcNow,
                OrderItemId = userReview.OrderItemId,
                Name = user.FullName,
                ReviewId = userReview.ReviewId
            };
        }


    }
}
