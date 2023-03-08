using Microsoft.EntityFrameworkCore;
using System.Linq;
using web_api_cosmetics_shop.Data;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.ProductOptionService
{
	public class ProductOptionService : IProductOptionService
	{
		private readonly CosmeticsShopContext _context;

		public ProductOptionService(CosmeticsShopContext context)
		{
			_context = context;
		}

		//------------ Add ------------
		public async Task<ProductOptionType> AddOptionsType(ProductOptionType optionsType)
		{
			await _context.ProductOptionTypes.AddAsync(optionsType);
			var result = await _context.SaveChangesAsync();
			if(result == 0)
			{
				return null!;
			}

			return optionsType;
		}

		public async Task<ProductOption> AddOption(ProductOption option)
		{
			await _context.ProductOptions.AddAsync(option);
			var result = await _context.SaveChangesAsync();
			if(result == 0)
			{
				return null!;
			}

			return option;
		}

		//------------ Get ------------
		// Get all Options Type
		public async Task<List<ProductOptionType>> GetAllOptionsTypes()
		{
			var types = await _context.ProductOptionTypes.ToListAsync();
			return types;
		}

		// Get Options Type
		public async Task<ProductOptionType> GetOptionsTypeById(int id)
		{
			var type = await _context.ProductOptionTypes.FirstOrDefaultAsync(t => t.OptionTypeId == id);
			return type!;
		}

		// Get Options of Option Type
		public async Task<List<ProductOption>> GetOptions(ProductOptionType optionsType)
		{
			var options = await _context.ProductOptions.Where(po => po.OptionTypeId == optionsType.OptionTypeId).ToListAsync();
			return options;
		}

		// Check exist Option Type name
		public async Task<bool> GetExistOptionTypeName(string optionsTypeName)
		{
			var isExistOptionTypeName = await _context.ProductOptionTypes.AnyAsync(pot => pot.Name == optionsTypeName);
			return isExistOptionTypeName;
		}

		//------------ Remove ------------
		public async Task<int> RemoveOptionsType(ProductOptionType optionsType)
		{
			_context.Remove(optionsType);
			var result = await _context.SaveChangesAsync();

			return result;
		}

		public async Task<int> RemoveOptions(ProductOptionType optionsType)
		{
			var options = await _context.ProductOptions
								.Where(po => po.OptionTypeId == optionsType.OptionTypeId)
								.ToListAsync();

			_context.RemoveRange(options);
			var result = await _context.SaveChangesAsync();

			return result;
		}


		//------------ Update ------------
		//public async Task<ProductOptionTypeDTO> UpdateProductOptions(ProductOptionTypeDTO productOptionTypeDTO)
		//{
		//	if(productOptionTypeDTO.Options == null)
		//	{
		//		return null;
		//	}

		//	// Get existing Options Type
		//	var existOptionsType = await _context.ProductOptionTypes.FirstOrDefaultAsync(opt => opt.OptionTypeId == productOptionTypeDTO.OptionTypeId);
		//	if(existOptionsType == null)
		//	{
		//		return null;
		//	}

		//	// Updating Option Type
		//	if(existOptionsType.Name != productOptionTypeDTO.Name)
		//	{
		//		existOptionsType.Name = productOptionTypeDTO.Name;
		//		var optionsTypeResult = await _context.SaveChangesAsync();
		//	}

		//	// Get existing Options
		//	var existOptions = await (from option in _context.ProductOptions
		//						where option.OptionTypeId == existOptionsType.OptionTypeId
		//						select option).ToListAsync();

		//	// Removing old Options
		//	_context.RemoveRange(existOptions);
		//	var removeOptionsResult = await _context.SaveChangesAsync();

		//	// Adding new Options
		//          foreach (var item in productOptionTypeDTO.Options)
		//          {
		//		var newOption = new ProductOption()
		//		{
		//			OptionTypeId = existOptionsType.OptionTypeId,
		//			Name = item.Name,
		//			Value = item.Value
		//		};

		//		await _context.ProductOptions.AddAsync(newOption);
		//		var optionResult = await _context.SaveChangesAsync();
		//		// Get created Option id
		//		item.ProductOptionId = newOption.ProductOptionId;
		//		if(optionResult == 0)
		//		{
		//			return null;
		//		}
		//          }

		//          return productOptionTypeDTO;
		//}

		public async Task<ProductOptionType> UpdateOptionsType(ProductOptionType optionsType)
		{
			var existOptionsType = await GetOptionsTypeById(optionsType.OptionTypeId);
			if(existOptionsType == null)
			{
				return null!;
			}

			existOptionsType.Name = optionsType.Name;

			var result = await _context.SaveChangesAsync();
			if(result == 0)
			{
				return null!;
			}

			return optionsType;
		}
	}
}
