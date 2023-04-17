using Microsoft.EntityFrameworkCore;
using web_api_cosmetics_shop.Data;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.AddressService
{
	public class AddressService : IAddressService
	{
		private readonly CosmeticsShopContext _context;
		public AddressService(CosmeticsShopContext context)
		{
			_context = context;
		}

		// Create address
		public async Task<Address> AddAddress(Address address)
		{
			await _context.AddAsync(address);
			var result = await _context.SaveChangesAsync();
			if(result == 0)
			{
				return null!;
			}

			return address;
		}

		// Get address
		public async Task<Address> GetAddress(int addressId)
		{
			var address = await _context.Addresses.FirstOrDefaultAsync(a => a.AddressId == addressId);

			return address!;
		}

		public async Task<List<Address>> GetUserAddresses(string userId)
		{
			var userAddresses = await _context.Addresses.Where(a => a.UserId == userId && a.IsDisplay != false).ToListAsync();
			return userAddresses;
		}

		public async Task<List<Address>> GetAllAddresses()
		{
			var allAddress = await _context.Addresses.ToListAsync();
			return allAddress;
		}

        public async Task<bool> IsHasOrder(Address address)
		{
			var orderCount = await  (from add in _context.Addresses
                              join ord in _context.ShopOrders on add.AddressId equals ord.AddressId
							  where add.AddressId == address.AddressId
                              select add).CountAsync();
			if(orderCount == 0)
			{
				return false;
			}

			return true;
		}

		// Remove address
        public async Task<int> RemoveAddress(Address address)
		{
			_context.Remove(address);
			var result = await _context.SaveChangesAsync();
			return result;
		}

		// Update address
		public async Task<Address> UpdateAddress(Address address)
		{
			var existAddress = await GetAddress(address.AddressId);

			if(existAddress == null)
			{
				return null!;
			}

			existAddress.FullName = address.FullName;
			existAddress.City = address.City;
			existAddress.District = address.District;
			existAddress.Ward = address.Ward;
			existAddress.AddressLine = address.AddressLine;
			existAddress.PhoneNumber = address.PhoneNumber;
			existAddress.IsDefault = address.IsDefault;
			existAddress.IsDisplay = address.IsDisplay;

			var result = await _context.SaveChangesAsync();
			if(result == 0)
			{
				return null!;
			}

			return existAddress;
		}

		// Convert to DTO
        public AddressDTO ConvertToAddressDto(Address address)
        {
            return new AddressDTO()
            {
                AddressId = address.AddressId,
                UserId = address.UserId,
                FullName = address.FullName,
                City = address.City,
                District = address.District,
                Ward = address.Ward,
                AddressLine = address.AddressLine,
                PhoneNumber = address.PhoneNumber,
                IsDefault = address.IsDefault,
				CreateAt = address.CreateAt
            };
        }
    }
}
