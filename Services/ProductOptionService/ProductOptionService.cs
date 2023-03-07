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

		public async Task<ProductOptionTypeDTO> AddProductOptions(ProductOptionTypeDTO productOptionTypeDTO)
		{
			if (productOptionTypeDTO.Options == null)
			{
				return null;
			}

			// Create new ProductOptionType
			var newProductOptionType = new ProductOptionType()
			{
				Name = productOptionTypeDTO.Name
			};

			await _context.ProductOptionTypes.AddAsync(newProductOptionType);
			int createProductOptionTypeResult = await _context.SaveChangesAsync();
			// Get Created OptionType Id
			productOptionTypeDTO.OptionTypeId = newProductOptionType.OptionTypeId;
			if (createProductOptionTypeResult == 0)
			{
				// If ProductOptionTypes creation fails, return null
				return null;
			}

			// Else create ProductOptions
			foreach(var productOption in productOptionTypeDTO.Options)
			{
				var newProductOption = new ProductOption()
				{
					OptionTypeId = newProductOptionType.OptionTypeId,
					Name = productOption.Name,
					Value = productOption.Value
				};

				await _context.ProductOptions.AddAsync(newProductOption);
				int createOptionResult = await _context.SaveChangesAsync();
				// Get Created Option Id
				productOption.ProductOptionId = newProductOption.ProductOptionId;
				if (createOptionResult == 0)
				{
					return null;
				}
			}

			return productOptionTypeDTO;
		}

		public async Task<ProductOptionTypeDTO> GetProductOption(int productOptionTypeId)
		{
			var optionType = await _context.ProductOptionTypes.FirstOrDefaultAsync(pot => pot.OptionTypeId == productOptionTypeId);
			if(optionType == null)
			{
				return null;
			}

			var options = await (from option in _context.ProductOptions
						   where option.OptionTypeId == productOptionTypeId
						   select new ProductOptionDTO()
						   {
							   ProductOptionId = option.ProductOptionId,
							   Name = option.Name,
							   Value = option.Value
						   }).ToListAsync();

			return new ProductOptionTypeDTO()
			{
				OptionTypeId = optionType.OptionTypeId,
				Name = optionType.Name,
				Options = options
			};
		}

		// Get All ProductOptions
		public async Task<List<ProductOptionTypeDTO>> GetProductOptions()
		{
			// Get Product Option Types
			var optionTypes = await (from optionType in _context.ProductOptionTypes
									 select optionType).ToListAsync();

			// Get Options of Type
			List<ProductOptionTypeDTO> listOptions = new List<ProductOptionTypeDTO>();
			foreach(var optionType in optionTypes )
			{
				var options = await GetProductOption(optionType.OptionTypeId);
				listOptions.Add(options);
			}

			return listOptions;
		}

		public async Task<ProductOptionTypeDTO> RemoveProductOptions(int productOptionTypeId)
		{
			var optionType = await _context.ProductOptionTypes.FirstOrDefaultAsync(pot => pot.OptionTypeId == productOptionTypeId);
			if (optionType == null)
			{
				return null;
			}

			var options = await (from option in _context.ProductOptions
								 where option.OptionTypeId == productOptionTypeId
								 select option).ToListAsync();

			// Return removed value
			var optionsAsProductOptionDTO = options.Select(o => new ProductOptionDTO()
			{
				ProductOptionId = o.ProductOptionId,
				Name = o.Name,
				Value = o.Value
			}).ToList();
			var removedOptionType = new ProductOptionTypeDTO()
			{
				OptionTypeId = optionType.OptionTypeId,
				Name = optionType.Name,
				Options = optionsAsProductOptionDTO
			};

			// Removing Product Options
			_context.ProductOptions.RemoveRange(options);
			var removeProductOpResult = await _context.SaveChangesAsync();
			if (removeProductOpResult == 0)
			{
				return null;
			}

			// Removing Product Option Type
			_context.ProductOptionTypes.Remove(optionType);
			var result = await _context.SaveChangesAsync();
			if (result == 0)
			{
				return null;
			}

			return removedOptionType;
		}

		public async Task<ProductOptionTypeDTO> UpdateProductOptions(ProductOptionTypeDTO productOptionTypeDTO)
		{
			if(productOptionTypeDTO.Options == null)
			{
				return null;
			}

			// Get existing Options Type
			var existOptionsType = await _context.ProductOptionTypes.FirstOrDefaultAsync(opt => opt.OptionTypeId == productOptionTypeDTO.OptionTypeId);
			if(existOptionsType == null)
			{
				return null;
			}

			// Updating Option Type
			if(existOptionsType.Name != productOptionTypeDTO.Name)
			{
				existOptionsType.Name = productOptionTypeDTO.Name;
				var optionsTypeResult = await _context.SaveChangesAsync();
			}

			// Get existing Options
			var existOptions = await (from option in _context.ProductOptions
								where option.OptionTypeId == existOptionsType.OptionTypeId
								select option).ToListAsync();
			
			// Removing old Options
			_context.RemoveRange(existOptions);
			var removeOptionsResult = await _context.SaveChangesAsync();

			// Adding new Options
            foreach (var item in productOptionTypeDTO.Options)
            {
				var newOption = new ProductOption()
				{
					OptionTypeId = existOptionsType.OptionTypeId,
					Name = item.Name,
					Value = item.Value
				};

				await _context.ProductOptions.AddAsync(newOption);
				var optionResult = await _context.SaveChangesAsync();
				// Get created Option id
				item.ProductOptionId = newOption.ProductOptionId;
				if(optionResult == 0)
				{
					return null;
				}
            }

            return productOptionTypeDTO;
		}

		public async Task<bool> GetExistOptionTypeName(string optionTypeName)
		{
			var isExistOptionTypeName = await _context.ProductOptionTypes.AnyAsync(pot => pot.Name == optionTypeName);
			return isExistOptionTypeName;
		}
	}
}
