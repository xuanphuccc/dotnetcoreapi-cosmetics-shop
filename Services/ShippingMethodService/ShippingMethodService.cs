using Microsoft.EntityFrameworkCore;
using web_api_cosmetics_shop.Data;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.ShippingMethodService
{
    public class ShippingMethodService : IShippingMethodService
    {
        private readonly CosmeticsShopContext _context;
        public ShippingMethodService(CosmeticsShopContext context)
        {
            _context = context;
        }

        public async Task<ShippingMethod> AddShippingMethod(ShippingMethod shippingMethod)
        {
            await _context.ShippingMethods.AddAsync(shippingMethod);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                throw new Exception("cannot create shipping method");
            }

            return shippingMethod;
        }

        public async Task<List<ShippingMethod>> GetAllShippingMethods()
        {
            var shippingMethods = await _context.ShippingMethods.ToListAsync();

            return shippingMethods;
        }

        public async Task<ShippingMethod> GetShippingMethod(int shippingMethodId)
        {
            var shippingMethod = await _context.ShippingMethods.FirstOrDefaultAsync(s => s.ShippingMethodId == shippingMethodId);

            return shippingMethod!;
        }

        public async Task<int> RemoveShippingMethod(ShippingMethod shippingMethod)
        {
            _context.Remove(shippingMethod);
            var result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                throw new Exception("cannot delete shipping method");
            }

            return result;
        }

        public async Task<ShippingMethod> UpdateShippingMethod(ShippingMethod shippingMethod)
        {
            var existShippingMethod = await GetShippingMethod(shippingMethod.ShippingMethodId);
            if (existShippingMethod == null)
            {
                throw new Exception("shipping method not found");
            }

            existShippingMethod.Name = shippingMethod.Name;
            existShippingMethod.Price = shippingMethod.Price;

            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                throw new Exception("cannot update shipping method");
            }

            return existShippingMethod;
        }


        // Filter
        public IQueryable<ShippingMethod> FilterAllShippingMethods()
        {
            var shippingMethods = _context.ShippingMethods.AsQueryable();

            return shippingMethods;
        }
        public IQueryable<ShippingMethod> FilterSearch(IQueryable<ShippingMethod> shippingMethods, string search)
        {
            shippingMethods = shippingMethods.Where(s => s.Name.ToLower().Contains(search.ToLower()));

            return shippingMethods;
        }
        public IQueryable<ShippingMethod> FilterSortByCreationTime(IQueryable<ShippingMethod> shippingMethods, bool isDesc = true)
        {
            if (isDesc)
            {
                shippingMethods = shippingMethods.OrderByDescending(s => s.CreateAt);
            }
            else
            {
                shippingMethods = shippingMethods.OrderBy(s => s.CreateAt);
            }

            return shippingMethods;
        }
        public IQueryable<ShippingMethod> FilterSortByName(IQueryable<ShippingMethod> shippingMethods, bool isDesc = false)
        {
            if (isDesc)
            {
                shippingMethods = shippingMethods.OrderByDescending(s => s.Name);
            }
            else
            {
                shippingMethods = shippingMethods.OrderBy(s => s.Name);
            }

            return shippingMethods;
        }
        public IQueryable<ShippingMethod> FilterSortByPrice(IQueryable<ShippingMethod> shippingMethods, bool isDesc = false)
        {
            if (isDesc)
            {
                shippingMethods = shippingMethods.OrderByDescending(s => s.Price);
            }
            else
            {
                shippingMethods = shippingMethods.OrderBy(s => s.Price);
            }

            return shippingMethods;
        }
    }
}
