using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.ProviderService
{
    public interface IProviderService
    {
        // Get
        Task<List<Provider>> GetAllProviders();
        Task<Provider> GetProvider(int providerId);
        IQueryable<Provider> GetAllProvidersQueryable();

        // Add
        Task<Provider> AddProvider(Provider provider);

        // Update
        Task<Provider> UpdateProvider(Provider provider);

        // Delete
        Task<int> RemoveProvider(Provider provider);
    }
}
