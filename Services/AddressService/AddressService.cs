﻿using Microsoft.EntityFrameworkCore;
using web_api_cosmetics_shop.Data;
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

		public async Task<Address> GetAddress(int addressId)
		{
			var address = await _context.Addresses.FirstOrDefaultAsync(a => a.AddressId == addressId);
			return address!;
		}

		public async Task<List<Address>> GetUserAddresses(string userId)
		{
			var userAddresses = await _context.Addresses.Where(a => a.UserId == userId).ToListAsync();
			return userAddresses;
		}

		public async Task<List<Address>> GetAllAddresses()
		{
			var allAddress = await _context.Addresses.ToListAsync();
			return allAddress;
		}

		public async Task<int> RemoveAddress(Address address)
		{
			_context.Remove(address);
			var result = await _context.SaveChangesAsync();
			return result;
		}

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

			var result = await _context.SaveChangesAsync();
			if(result == 0)
			{
				return null!;
			}

			return existAddress;
		}
	}
}
