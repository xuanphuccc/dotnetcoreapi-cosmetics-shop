using web_api_cosmetics_shop.Models.DTO;

namespace web_api_cosmetics_shop.Services.ProductOptionService
{
	public interface IProductOptionService
	{
		Task<List<ProductOptionTypeDTO>> GetProductOptions();
		Task<ProductOptionTypeDTO> GetProductOption(int productOptionTypeId);
		Task<bool> GetExistOptionTypeName(string optionTypeName);
		Task<ProductOptionTypeDTO> AddProductOptions(ProductOptionTypeDTO productOptionTypeDTO);
		Task<ProductOptionTypeDTO> UpdateProductOptions(ProductOptionTypeDTO productOptionTypeDTO);
		Task<ProductOptionTypeDTO> RemoveProductOptions(int productOptionTypeId);
	}
}
