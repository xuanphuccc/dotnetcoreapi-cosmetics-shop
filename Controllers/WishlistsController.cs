﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.ProductService;
using web_api_cosmetics_shop.Services.UserService;
using web_api_cosmetics_shop.Services.WishlistService;

namespace web_api_cosmetics_shop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistsController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        public WishlistsController(
            IWishlistService wishlistService,
            IProductService productService,
            IUserService userService)
        {
            _wishlistService = wishlistService;
            _productService = productService;
            _userService = userService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllWishlists()
        {
            var allWishlists = await _wishlistService.GetAllWishlists();

            List<WishlistDTO> wishlistDtos = new List<WishlistDTO>();
            foreach (var item in allWishlists)
            {
                var productId = item.ProductId != null ? item.ProductId.Value : 0;
                var product = await _productService.GetProductById(productId);
                var productDto = await _productService.ConvertToProductDtoAsync(product);

                wishlistDtos.Add(_wishlistService.ConvertToWishlistDto(item, productDto));
            }

            return Ok(new ResponseDTO()
            {
                Data = wishlistDtos,
            });
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> GetUserWishlist([FromRoute] string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var userWishlist = await _wishlistService.GetUserWishlist(id);

            List<WishlistDTO> wishlistDtos = new();
            foreach (var item in userWishlist)
            {
                var productId = item.ProductId != null ? item.ProductId.Value : 0;
                var product = await _productService.GetProductById(productId);
                var productDto = await _productService.ConvertToProductDtoAsync(product);

                wishlistDtos.Add(_wishlistService.ConvertToWishlistDto(item, productDto));
            }

            return Ok(new ResponseDTO()
            {
                Data = wishlistDtos,
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddWishlist([FromBody] WishlistDTO wishlistDto)
        {
            if (wishlistDto == null)
            {
                return BadRequest();
            }

            // Logged user
            var currentIdentityUser = _userService.GetCurrentUser(HttpContext.User);
            if (currentIdentityUser == null)
            {
                return NotFound(new ErrorDTO() { Title = "user not found", Status = 404 });
            }

            // Exist user from database
            var currentUser = await _userService.GetUserByUserName(currentIdentityUser.UserName);
            if (currentUser == null)
            {
                return NotFound(new ErrorDTO() { Title = "user not found", Status = 404 });
            }

            // Check exist product
            var product = await _productService.GetProductById(wishlistDto.ProductId!.Value);
            if (product == null)
            {
                return NotFound(new ErrorDTO() { Title = "product not found", Status = 400 });
            }

            // Check exist wishlist
            var existWishlist = await _wishlistService.ExistWishlist(currentUser.UserId, product.ProductId);
            if (existWishlist != null)
            {
                return BadRequest(new ErrorDTO() { Title = "wishlist already exist", Status = 400 });
            }

            // Add wishlist item
            try
            {
                var newWishlist = new Wishlist()
                {
                    UserId = currentUser.UserId,
                    ProductId = wishlistDto.ProductId,
                    CreateAt = DateTime.UtcNow,
                };

                var createdWishList = await _wishlistService.AddWishlist(newWishlist);

                return CreatedAtAction(nameof(GetUserWishlist),
                    new { id = createdWishList.UserId },
                    new ResponseDTO()
                    {
                        Data = _wishlistService.ConvertToWishlistDto(createdWishList),
                        Status = 201,
                    });
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }
        }

        [HttpDelete("{id?}")]
        public async Task<IActionResult> RemoveWishlist([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            // Get exist wishlist
            var existWishlist = await _wishlistService.GetWishlist(id.Value);
            if (existWishlist == null)
            {
                return NotFound(new ErrorDTO() { Title = "wishlist not found", Status = 404 });
            }

            // Remove wishlist
            try
            {
                await _wishlistService.RemoveWishlist(existWishlist);
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = _wishlistService.ConvertToWishlistDto(existWishlist)
            });
        }
    }
}
