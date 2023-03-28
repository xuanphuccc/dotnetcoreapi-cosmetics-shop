using Microsoft.EntityFrameworkCore;
using web_api_cosmetics_shop.Data;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.ProviderService
{
    public class ProviderService : IProviderService
    {
        private readonly CosmeticsShopContext _context;
        public ProviderService(CosmeticsShopContext context)
        {
            _context = context;
        }

        public async Task<Provider> AddProvider(Provider provider)
        {
            await _context.AddAsync(provider);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return null!;
            }

            return provider;
        }

        public async Task<List<Provider>> GetAllProviders()
        {
            var providers = await _context.Providers.ToListAsync();
            return providers;
        }

        public async Task<Provider> GetProvider(int providerId)
        {
            var provider = await _context.Providers
                .FirstOrDefaultAsync(p => p.ProviderId == providerId);

            return provider!;
        }

        public IQueryable<Provider> GetAllProvidersQueryable()
        {
            var provider = _context.Providers;
            return provider;
        }

        public async Task<int> RemoveProvider(Provider provider)
        {
            _context.Remove(provider);
            var result = await _context.SaveChangesAsync();
            return result;
        }

        public async Task<Provider> UpdateProvider(Provider provider)
        {
            var existProvider = await GetProvider(provider.ProviderId);
            if (existProvider == null)
            {
                return null!;
            }

            existProvider.Name = provider.Name;
            var result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                return null!;
            }

            return existProvider;
        }
    }
}
