using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.ProviderService
{
    public interface IProviderService
    {
        // Get
        Task<List<Provider>> GetAllProviders();
        Task<Provider> GetProvider(int providerId);

        // Add
        Task<Provider> AddProvider(Provider provider);

        // Update
        Task<Provider> UpdateProvider(Provider provider);

        // Delete
        Task<int> RemoveProvider(Provider provider);

        // Filter
        IQueryable<Provider> FilterAllProviders();
        IQueryable<Provider> FilterSearch(IQueryable<Provider> providers, string search);
        IQueryable<Provider> FilterSortByCreationTime(IQueryable<Provider> providers, bool isDesc = true);
        IQueryable<Provider> FilterSortByName(IQueryable<Provider> providers, bool isDesc = false);
    }
}
