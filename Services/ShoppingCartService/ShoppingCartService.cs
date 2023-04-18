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
				throw new Exception("cannot create shopping cart");
			}

			return shoppingCart;
		}

		public async Task<ShoppingCartItem> AddShoppingCartItem(ShoppingCartItem shoppingCartItem)
		{
			await _context.ShoppingCartItems.AddAsync(shoppingCartItem);
			var result = await _context.SaveChangesAsync();

			if (result == 0)
			{
                throw new Exception("cannot create shopping cart item");
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

		public async Task<List<ShoppingCartItem>> GetAllShoppingCartItems(ShoppingCart shoppingCart)
		{
			var shoppingCartItems = await _context.ShoppingCartItems
										.Where(ci => ci.CartId == shoppingCart.CartId)
										.ToListAsync();

			return shoppingCartItems;
		}

		public async Task<ShoppingCartItem> GetShoppingCartItem(int cartItemId)
		{
			var shoppingCartItem = await _context.ShoppingCartItems
									.FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);

			return shoppingCartItem!;
		}

        public async Task<ShoppingCartItem> IsExistProductItem(ShoppingCart shoppingCart, int productItemId)
        {
            var isExist = await _context.ShoppingCartItems.FirstOrDefaultAsync(
                                        ci => ci.ProductItemId == productItemId &&
                                        ci.CartId == shoppingCart.CartId);

            return isExist!;
        }


        // ---------------- Remove ----------------
        public async Task<int> RemoveShoppingCartItem(ShoppingCartItem shoppingCartItem)
		{
			_context.Remove(shoppingCartItem);
			var result = await _context.SaveChangesAsync();

			if(result == 0)
			{
                throw new Exception("cannot delete shopping cart item");
            }

			return result;
		}

		// ---------------- Update ----------------
		public async Task<ShoppingCartItem> UpdateShoppingCartItem(ShoppingCartItem shoppingCartItem)
		{
			var existShoppingCartItem = await _context.ShoppingCartItems.FirstOrDefaultAsync(ci => ci.CartItemId == shoppingCartItem.CartItemId);
			if (existShoppingCartItem == null)
			{
                throw new Exception("shopping cart item not found");
            }

			existShoppingCartItem.Qty = shoppingCartItem.Qty;

			var result = await _context.SaveChangesAsync();

			if(result == 0)
			{
                throw new Exception("cannot update shopping cart item");
            }

			return shoppingCartItem;
		}

		public async Task<int> IncreaseQtyOfShoppingCartItem(ShoppingCartItem shoppingCartItem, int qty)
		{
			var existShoppingCartItem = await _context.ShoppingCartItems.FirstOrDefaultAsync(ci => ci.CartItemId == shoppingCartItem.CartItemId);
			if (existShoppingCartItem == null)
			{
                throw new Exception("shopping cart item not found");
            }

			existShoppingCartItem.Qty += qty;

			var result = await _context.SaveChangesAsync();

			if (result == 0)
			{
                throw new Exception("cannot increase qty shopping cart item");
            }

			var resultQty = existShoppingCartItem.Qty;
			return resultQty;
		}
	}
}
