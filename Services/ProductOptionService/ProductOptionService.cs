﻿using Microsoft.EntityFrameworkCore;
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
            if (result == 0)
            {
                throw new Exception("cannot create options type");
            }

            return optionsType;
        }

        public async Task<ProductOption> AddOption(ProductOption option)
        {
            await _context.ProductOptions.AddAsync(option);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                throw new Exception("cannot create option");
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

        // Get one Option by id
        public async Task<ProductOption> GetOption(int optionId)
        {
            var option = await _context.ProductOptions.FirstOrDefaultAsync(po => po.ProductOptionId == optionId);

            return option!;
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

            if(result == 0)
            {
                throw new Exception("cannot delete options type");
            }

            return result;
        }

        public async Task<int> RemoveOption(ProductOption option)
        {
            _context.Remove(option);
            var result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                throw new Exception("cannot delete option");
            }

            return result;
        }

        //------------ Update ------------
        public async Task<ProductOptionType> UpdateOptionsType(ProductOptionType optionsType)
        {
            var existOptionsType = await GetOptionsTypeById(optionsType.OptionTypeId);
            if (existOptionsType == null)
            {
                throw new Exception("options type not found");
            }

            existOptionsType.Name = optionsType.Name;

            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                throw new Exception("cannot update options type");
            }

            return optionsType;
        }

        public async Task<ProductOption> UpdateOption(ProductOption option)
        {
            var existOption = await _context.ProductOptions.FirstOrDefaultAsync(o => o.ProductOptionId == option.ProductOptionId);
            if (existOption == null)
            {
                throw new Exception("option not found");
            }

            existOption.Name = option.Name;
            existOption.Value = option.Value;

            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                throw new Exception("cannot update option");
            }

            return option;
        }


        // Filter
        public IQueryable<ProductOptionType> FilterAllProductOptionTypes()
        {
            var productOptionTypes = _context.ProductOptionTypes.AsQueryable();

            return productOptionTypes;
        }

        public IQueryable<ProductOptionType> FilterSearch(IQueryable<ProductOptionType> productOptionTypes, string search)
        {
            productOptionTypes = productOptionTypes.Where(t => t.Name.ToLower().Contains(search.ToLower()));

            return productOptionTypes;
        }

        public IQueryable<ProductOptionType> FilterSortByName(IQueryable<ProductOptionType> productOptionTypes, bool isDesc = false)
        {
            if (isDesc)
            {
                productOptionTypes = productOptionTypes.OrderByDescending(t => t.Name);
            }
            else
            {
                productOptionTypes = productOptionTypes.OrderBy(t => t.Name);
            }

            return productOptionTypes;
        }
    }
}
