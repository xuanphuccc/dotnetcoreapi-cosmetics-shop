using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.PromotionService
{
    public interface IPromotionService
    {
        Task<List<Promotion>> GetPromotionsAsync();
        Task<Promotion> GetPromotionByIdAsync(int id);
        Task<Promotion> AddPromotionAsync(Promotion promotion);
        Task<Promotion> UpdatePromotionAsync(Promotion promotion);
        Task<Promotion> RemovePromotionAsync(Promotion promotion);
    }
}
