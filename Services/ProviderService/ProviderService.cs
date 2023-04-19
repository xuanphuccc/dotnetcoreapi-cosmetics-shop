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
                throw new Exception("cannot create provider");
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

        public async Task<int> RemoveProvider(Provider provider)
        {
            _context.Remove(provider);
            var result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                throw new Exception("cannot delete provider");
            }

            return result;
        }

        public async Task<Provider> UpdateProvider(Provider provider)
        {
            var existProvider = await GetProvider(provider.ProviderId);
            if (existProvider == null)
            {
                throw new Exception("provider not found");
            }

            existProvider.Name = provider.Name;
            var result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                throw new Exception("cannot update provider");
            }

            return existProvider;
        }

        // Filter
        public IQueryable<Provider> FilterAllProviders()
        {
            var providers = _context.Providers.AsQueryable();

            return providers;
        }
        public IQueryable<Provider> FilterSearch(IQueryable<Provider> providers, string search)
        {
            providers = providers.Where(p => p.Name.ToLower().Contains(search.ToLower()));

            return providers;
        }
        public IQueryable<Provider> FilterSortByCreationTime(IQueryable<Provider> providers, bool isDesc = true)
        {
            if (isDesc)
            {
                providers = providers.OrderByDescending(p => p.CreateAt);
            }
            else
            {
                providers = providers.OrderBy(p => p.CreateAt);
            }

            return providers;
        }
        public IQueryable<Provider> FilterSortByName(IQueryable<Provider> providers, bool isDesc = false)
        {
            if (isDesc)
            {
                providers = providers.OrderByDescending(p => p.Name);
            }
            else
            {
                providers = providers.OrderBy(p => p.Name);
            }

            return providers;
        }
    }
}
