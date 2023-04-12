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
			if(result == 0)
			{
				return null!;
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

			return result;
		}

		public async Task<ShippingMethod> UpdateShippingMethod(ShippingMethod shippingMethod)
		{
			var existShippingMethod = await GetShippingMethod(shippingMethod.ShippingMethodId);
			if(existShippingMethod == null)
			{
				return null!;
			}

			existShippingMethod.Name = shippingMethod.Name;
			existShippingMethod.Price = shippingMethod.Price;

			var result = await _context.SaveChangesAsync();
			if(result == 0)
			{
				return null!;
			}

			return existShippingMethod;
		}
	}
}
