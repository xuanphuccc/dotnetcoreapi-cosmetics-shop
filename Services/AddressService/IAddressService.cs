using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.AddressService
{
	public interface IAddressService
	{
		// Get
		Task<List<Address>> GetAllAddresses();
		Task<List<Address>> GetUserAddresses(string userId);
		Task<Address> GetAddress(int addressId);
		Task<bool> IsHasOrder(Address address);


		// Add
		Task<Address> AddAddress(Address address);

		// Update
		Task<Address> UpdateAddress(Address address);

		// Delete
		Task<int> RemoveAddress(Address address);

		// Convert to DTO
		AddressDTO ConvertToAddressDto(Address address);

    }
}
