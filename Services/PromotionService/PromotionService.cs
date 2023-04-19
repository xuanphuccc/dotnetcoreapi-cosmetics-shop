using web_api_cosmetics_shop.Data;
using web_api_cosmetics_shop.Models.Entities;
using Microsoft.EntityFrameworkCore;
using web_api_cosmetics_shop.Models.DTO;

namespace web_api_cosmetics_shop.Services.PromotionService
{
    public class PromotionService : IPromotionService
    {
        private readonly CosmeticsShopContext _context;

        public PromotionService(CosmeticsShopContext context)
        {
            _context = context;
        }

        // Get all promotions
        public async Task<List<Promotion>> GetAllPromotions()
        {
            var allPromotions = await _context.Promotions.ToListAsync();
            return allPromotions;
        }

        // Get promotion by id
        public async Task<Promotion> GetPromotion(int id)
        {
            var promotion = await _context.Promotions.FirstOrDefaultAsync(p => p.PromotionId == id);
            if (promotion == null)
            {
                return null!;
            }

            return promotion;
        }

        // Add promotion
        public async Task<Promotion> AddPromotion(Promotion promotion)
        {
            await _context.Promotions.AddAsync(promotion);
            var result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                throw new Exception("cannot create promotion");
            }

            return promotion;
        }

        // Remove promotion
        public async Task<Promotion> RemovePromotion(Promotion promotion)
        {
            _context.Promotions.Remove(promotion);
            var result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                throw new Exception("cannot delete promotion");
            }
            return promotion;
        }

        // Update promotion
        public async Task<Promotion> UpdatePromotion(Promotion promotion)
        {
            var existPromotion = await _context.Promotions.FirstOrDefaultAsync(p => p.PromotionId == promotion.PromotionId);

            if (existPromotion == null)
            {
                throw new Exception("promotion not found");
            }

            existPromotion.Name = promotion.Name;
            existPromotion.Description = promotion.Description;
            existPromotion.DiscountRate = promotion.DiscountRate;
            existPromotion.StartDate = promotion.StartDate;
            existPromotion.EndDate = promotion.EndDate;

            var result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                throw new Exception("cannot update promotion");
            }

            return promotion;
        }

        public async Task<bool> GetExistPromotionName(string name)
        {
            var isExistName = await _context.Promotions.AnyAsync(p => p.Name.ToLower() == name.ToLower());
            return isExistName;
        }

        // Filter
        public IQueryable<Promotion> FilterAllPromotions()
        {
            var promotions = _context.Promotions.AsQueryable();
            return promotions;
        }

        public IQueryable<Promotion> FilterSearch(IQueryable<Promotion> promotions, string search)
        {
            promotions = promotions.Where(p => p.Name.ToLower().Contains(search.ToLower()));

            return promotions;
        }

        public IQueryable<Promotion> FilterSortByCreationTime(IQueryable<Promotion> promotions, bool isDesc = true)
        {
            if (isDesc)
            {
                promotions = promotions.OrderByDescending(p => p.CreateAt);
            }
            else
            {
                promotions = promotions.OrderBy(p => p.CreateAt);
            }

            return promotions;
        }
        public IQueryable<Promotion> FilterSortByName(IQueryable<Promotion> promotions, bool isDesc = false)
        {
            if (isDesc)
            {
                promotions = promotions.OrderByDescending(p => p.Name);
            }
            else
            {
                promotions = promotions.OrderBy(p => p.Name);
            }

            return promotions;
        }

        public IQueryable<Promotion> FilterSortByDiscountRate(IQueryable<Promotion> promotions, bool isDesc = false)
        {
            if (isDesc)
            {
                promotions = promotions.OrderByDescending(p => p.DiscountRate);
            }
            else
            {
                promotions = promotions.OrderBy(p => p.DiscountRate);
            }

            return promotions;
        }

        public IQueryable<Promotion> FilterByStatus(IQueryable<Promotion> promotions, string status)
        {
            if (status == "active")
            {
                promotions = promotions.Where(p => p.StartDate <= DateTime.UtcNow && DateTime.UtcNow <= p.EndDate);
            }
            else if (status == "expired")
            {
                promotions = promotions.Where(p => DateTime.UtcNow > p.EndDate);
            }
            else if (status == "comming")
            {
                promotions = promotions.Where(p => DateTime.UtcNow < p.StartDate);
            }

            return promotions;
        }

        // Convert to DTO
        public PromotionDTO ConvertToPromotionDto(Promotion promotion)
        {
            return new PromotionDTO()
            {
                PromotionId = promotion.PromotionId,
                Name = promotion.Name,
                Description = promotion.Description,
                DiscountRate = promotion.DiscountRate,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                CreateAt = promotion.CreateAt,
            };
        }
    }
}
