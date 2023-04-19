using Microsoft.EntityFrameworkCore;
using web_api_cosmetics_shop.Data;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.WishlistService
{
	public class WishlistService : IWishlistService
	{
		private readonly CosmeticsShopContext _context;
		public WishlistService(CosmeticsShopContext context)
		{
			_context = context;
		}

		public async Task<Wishlist> AddWishlist(Wishlist wishlist)
		{
			await _context.AddAsync(wishlist);
			var result = await _context.SaveChangesAsync();
			if(result == 0)
			{
				throw new Exception("cannot create wishlist");
			}

			return wishlist;
		}

		public async Task<List<Wishlist>> GetAllWishlists()
		{
			var allWishlists = await _context.Wishlists.ToListAsync();
			return allWishlists;
		}

		public async Task<List<Wishlist>> GetUserWishlist(string userId)
		{
			var userWishlist = await _context.Wishlists.Where(w => w.UserId == userId).ToListAsync();
			return userWishlist;
		}

		public async Task<Wishlist> GetWishlist(int wishlistId)
		{
			var wishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.WishlistId ==  wishlistId);
			return wishlist!;
		}

        public async Task<Wishlist> ExistWishlist(string userId, int productId)
		{
			var wishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

			return wishlist!;
		}


        public async Task<int> RemoveWishlist(Wishlist wishlist)
		{
			_context.Remove(wishlist);
			var result = await _context.SaveChangesAsync();

			if(result == 0)
			{
                throw new Exception("cannot delete wishlist");
            }

			return result;
		}

		// Convert DTO
		public WishlistDTO ConvertToWishlistDto(Wishlist wishlist, ProductDTO? product)
		{
			return new WishlistDTO()
			{
				WishlistId = wishlist.WishlistId,
				UserId = wishlist.UserId,
				ProductId = wishlist.ProductId,
				Product = product,
				CreateAt = wishlist.CreateAt
			};
		}
	}
}
