using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.WishlistService
{
	public interface IWishlistService
	{
		// Get
		Task<List<Wishlist>> GetAllWishlists();
		Task<List<Wishlist>> GetUserWishlist(string userId);
		Task<Wishlist> GetWishlist(int wishlistId);

		Task<Wishlist> ExistWishlist(string userId, int productId);

		// Add
		Task<Wishlist> AddWishlist(Wishlist wishlist);

		// Remove
		Task<int> RemoveWishlist(Wishlist wishlist);

		// Convert
		WishlistDTO ConvertToWishlistDto(Wishlist wishlist, ProductDTO? product = null);
	}
}
