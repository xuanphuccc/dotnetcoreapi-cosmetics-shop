using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.ShoppingCartService
{
	public interface IShoppingCartService
	{
		// Get
		Task<ShoppingCart> GetShoppingCart(string userId);
		Task<List<ShoppingCart>> GetAllShoppingCarts();
		Task<List<ShoppingCartItem>> GetShoppingCartItems(ShoppingCart shoppingCart);

		// Add
		Task<ShoppingCart> AddShoppingCart(ShoppingCart shoppingCart);
		Task<ShoppingCartItem> AddShoppingCartItem(ShoppingCartItem shoppingCartItem);

		// Update
		Task<ShoppingCartItem> UpdateShoppingCartItem(ShoppingCartItem shoppingCartItem);
		Task<ShoppingCartItem> IsExistProductItem(int productItemId);
		Task<int> IncreaseQtyOfShoppingCartItem(ShoppingCartItem shoppingCartItem, int qty);

		// Remove
		Task<int> RemoveShoppingCartItem(ShoppingCartItem shoppingCartItem);
	}
}
