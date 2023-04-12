using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.ProductOptionService
{
	public interface IProductOptionService
	{
		// Get
		Task<List<ProductOptionType>> GetAllOptionsTypes();
		Task<ProductOptionType> GetOptionsTypeById(int optionTypeId);
		Task<List<ProductOption>> GetOptions(ProductOptionType optionsType);
		Task<ProductOption> GetOption(int optionId);
		Task<bool> GetExistOptionTypeName(string optionsTypeName);

		// Add
		Task<ProductOptionType> AddOptionsType(ProductOptionType optionsType);
		Task<ProductOption> AddOption(ProductOption option);

		// Update
		Task<ProductOptionType> UpdateOptionsType(ProductOptionType optionsType);
		Task<ProductOption> UpdateOption(ProductOption option);

		// Remove
		Task<int> RemoveOptionsType(ProductOptionType optionsType);
		Task<int> RemoveOptions(ProductOptionType optionsType);
		Task<int> RemoveOption(ProductOption option);
	}
}
