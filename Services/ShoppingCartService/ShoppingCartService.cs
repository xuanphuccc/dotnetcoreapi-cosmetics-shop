using Microsoft.EntityFrameworkCore;
using web_api_cosmetics_shop.Data;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.ShoppingCartService
{
	public class ShoppingCartService : IShoppingCartService
	{
		private readonly CosmeticsShopContext _context;

		public ShoppingCartService(CosmeticsShopContext context) {
			_context = context;
		}

		// ---------------- Add ----------------
		public async Task<ShoppingCart> AddShoppingCart(ShoppingCart shoppingCart)
		{
			await _context.ShoppingCarts.AddAsync(shoppingCart);
			var result = await _context.SaveChangesAsync();
			if(result == 0)
			{
				return null!;
			}

			return shoppingCart;
		}

		public async Task<ShoppingCartItem> AddShoppingCartItem(ShoppingCartItem shoppingCartItem)
		{
			await _context.ShoppingCartItems.AddAsync(shoppingCartItem);
			var result = await _context.SaveChangesAsync();
			if (result == 0)
			{
				return null!;
			}

			return shoppingCartItem;
		}

		// ---------------- Get ----------------
		public async Task<List<ShoppingCart>> GetAllShoppingCarts()
		{
			var shoppingCarts = await _context.ShoppingCarts.ToListAsync();

			return shoppingCarts;
		}

		public async Task<ShoppingCart> GetShoppingCart(string userId)
		{
			var shoppingCart = await _context.ShoppingCarts.FirstOrDefaultAsync(s => s.UserId == userId);

			return shoppingCart!;
		}

		public async Task<List<ShoppingCartItem>> GetShoppingCartItems(ShoppingCart shoppingCart)
		{
			var shoppingCartItems = await _context.ShoppingCartItems
										.Where(ci => ci.CartId == shoppingCart.CartId)
										.ToListAsync();

			return shoppingCartItems;
		}

		// ---------------- Remove ----------------
		public async Task<int> RemoveShoppingCartItem(ShoppingCartItem shoppingCartItem)
		{
			_context.Remove(shoppingCartItem);
			var result = await _context.SaveChangesAsync();

			return result;
		}

		// ---------------- Update ----------------
		public async Task<ShoppingCartItem> UpdateShoppingCartItem(ShoppingCartItem shoppingCartItem)
		{
			var existShoppingCartItem = await _context.ShoppingCartItems.FirstOrDefaultAsync(ci => ci.CartItemId == shoppingCartItem.CartItemId);
			if (existShoppingCartItem == null)
			{
				return null!;
			}

			existShoppingCartItem.Qty = shoppingCartItem.Qty;

			var result = await _context.SaveChangesAsync();

			if(result == 0)
			{
				return null!;
			}

			return shoppingCartItem;
		}

		public async Task<int> IncreaseQtyOfShoppingCartItem(ShoppingCartItem shoppingCartItem, int qty)
		{
			var existShoppingCartItem = await _context.ShoppingCartItems.FirstOrDefaultAsync(ci => ci.CartItemId == shoppingCartItem.CartItemId);
			if (existShoppingCartItem == null)
			{
				return 0;
			}

			existShoppingCartItem.Qty = existShoppingCartItem.Qty + qty;

			var result = await _context.SaveChangesAsync();

			if (result == 0)
			{
				return 0;
			}

			var resultQty = existShoppingCartItem.Qty;
			return resultQty;
		}

		public async Task<ShoppingCartItem> IsExistProductItem(int productItemId)
		{
			var isExist = await _context.ShoppingCartItems.FirstOrDefaultAsync(ci => ci.ProductItemId == productItemId);

			return isExist!;
		}
	}
}
