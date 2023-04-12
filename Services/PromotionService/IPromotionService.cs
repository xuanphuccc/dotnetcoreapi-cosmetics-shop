using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.PromotionService
{
    public interface IPromotionService
    {
        // Get
        Task<List<Promotion>> GetAllPromotions();
        Task<Promotion> GetPromotion(int id);
        Task<bool> GetExistPromotionName(string name);

        // Add
        Task<Promotion> AddPromotion(Promotion promotion);

        // Update
        Task<Promotion> UpdatePromotion(Promotion promotion);

        // Remove
        Task<Promotion> RemovePromotion(Promotion promotion);

        // Filter
        IQueryable<Promotion> FilterAllPromotions();
        IQueryable<Promotion> FilterSearch(IQueryable<Promotion> promotions, string search);
        IQueryable<Promotion> FilterSortByCreationTime(IQueryable<Promotion> promotions, bool isDesc = true);
        IQueryable<Promotion> FilterSortByName(IQueryable<Promotion> promotions, bool isDesc = false);
        IQueryable<Promotion> FilterSortByDiscountRate(IQueryable<Promotion> promotions, bool isDesc = false);
        IQueryable<Promotion> FilterByStatus(IQueryable<Promotion> promotions, string status);
    }
}
