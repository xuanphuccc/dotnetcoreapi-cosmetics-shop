using web_api_cosmetics_shop.Data;
using web_api_cosmetics_shop.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace web_api_cosmetics_shop.Services.PromotionService
{
    public class PromotionService : IPromotionService
    {
        private readonly CosmeticsShopContext _context;

        public PromotionService(CosmeticsShopContext context) {
            _context = context;
        }

        public async Task<List<Promotion>> GetPromotionsAsync()
        {
            var allPromotions = await _context.Promotions.ToListAsync();
            return allPromotions;
        }

        public async Task<Promotion> GetPromotionByIdAsync(int id)
        {
            var promotion = await _context.Promotions.FirstOrDefaultAsync(p => p.PromotionId == id);

            if(promotion == null)
            {
                return null;
            }

            return promotion;
        }

        public async Task<Promotion> AddPromotionAsync(Promotion promotion)
        {
            await _context.Promotions.AddAsync(promotion);
            var result = await _context.SaveChangesAsync();

            if(result == 0)
            {
                return null;
            }

            return promotion;
        }

        public async Task<Promotion> RemovePromotionAsync(Promotion promotion)
        {
            _context.Promotions.Remove(promotion);
            var result = await _context.SaveChangesAsync();

            if(result == 0)
            {
                return null;
            }
            return promotion;
        }

        public async Task<Promotion> UpdatePromotionAsync(Promotion promotion)
        {
            var existPromotion = await _context.Promotions.FirstOrDefaultAsync(p => p.PromotionId == promotion.PromotionId);

            if (existPromotion == null)
            {
                return null;
            }

            existPromotion.Name = promotion.Name;
            existPromotion.Description = promotion.Description;
            existPromotion.DiscountRate = promotion.DiscountRate;
            existPromotion.StartDate = promotion.StartDate;
            existPromotion.EndDate = promotion.EndDate;

            var result = await _context.SaveChangesAsync();

            if(result == 0)
            {
                return null;
            }

            return promotion;
        }
    }
}
