using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.AddressService;
using web_api_cosmetics_shop.Services.OrderStatusService;
using web_api_cosmetics_shop.Services.PaymentMethodService;
using web_api_cosmetics_shop.Services.ProductService;
using web_api_cosmetics_shop.Services.ShippingMethodService;
using web_api_cosmetics_shop.Services.ShopOrderService;
using web_api_cosmetics_shop.Services.UserService;

namespace web_api_cosmetics_shop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopOrdersController : ControllerBase
    {
        private readonly IShopOrderService _shopOrderService;
        private readonly IProductService _productService;
        private readonly IShippingMethodService _shippingMethodService;
        private readonly IAddressService _addressService;
        private readonly IUserService _userService;
        private readonly IOrderStatusService _orderStatusService;
        private readonly IPaymentMethodService _paymentMethodService;
        public ShopOrdersController(
            IShopOrderService shopOrderService,
            IProductService productService,
            IShippingMethodService shippingMethodService,
            IAddressService addressService,
            IUserService userService,
            IOrderStatusService orderStatusService,
            IPaymentMethodService paymentMethodService)
        {
            _shopOrderService = shopOrderService;
            _productService = productService;
            _shippingMethodService = shippingMethodService;
            _addressService = addressService;
            _userService = userService;
            _orderStatusService = orderStatusService;
            _paymentMethodService = paymentMethodService;
        }

        [NonAction]
        private async Task<ShopOrderDTO> ConvertToShopOrderDto(ShopOrder shopOrder)
        {
            var shopOrderItems = await _shopOrderService.GetOrderItems(shopOrder);

            List<OrderItemDTO> orderItemsDto = new();
            foreach (var item in shopOrderItems)
            {
                // Get product item
                ProductItem productItem = new();
                if (item.ProductItemId.HasValue)
                {
                    productItem = await _productService.GetItem(item.ProductItemId.Value);
                }

                // Get product
                Product product = new();
                if (productItem != null && productItem.ProductId.HasValue)
                {
                    product = await _productService.GetProductById(productItem.ProductId.Value);
                }

                var productDto = await _productService.ConvertToProductDtoAsync(product ?? new Product(), productItem != null ? productItem.ProductItemId : 0);

                orderItemsDto.Add(new OrderItemDTO()
                {
                    OrderItemId = item.OrderItemId,
                    Qty = item.Qty,
                    Price = item.Price,
                    DiscountRate = item.DiscountRate,
                    OrderId = item.OrderId,
                    ProductItemId = item.ProductItemId != null ? item.ProductItemId.Value : 0,
                    Product = productDto
                });
            }

            // Get order address
            Address address = new();
            if (shopOrder.AddressId.HasValue)
            {
                address = await _addressService.GetAddress(shopOrder.AddressId.Value);
            }

            AddressDTO addressDto = new();
            if (address != null)
            {
                addressDto = _addressService.ConvertToAddressDto(address);
            }

            // Get user
            AppUser appUser = new();
            if (!string.IsNullOrEmpty(shopOrder.UserId))
            {
                appUser = await _userService.GetUserById(shopOrder.UserId);
            }

            AppUserDTO appUserDto = new();
            if (appUser != null)
            {
                appUserDto = _userService.ConvertToAppUserDto(appUser);
            }

            // Get payment method
            PaymentMethod paymentMethod = new();
            if (shopOrder.PaymentMethodId.HasValue)
            {
                paymentMethod = await _paymentMethodService.GetPaymentMethod(shopOrder.PaymentMethodId.Value);
            }

            PaymentMethodDTO paymentMethodDto = new();
            if(paymentMethod != null)
            {
                paymentMethodDto = _paymentMethodService.ConvertToPaymentMethodDto(paymentMethod);
            }


            return new ShopOrderDTO()
            {
                OrderId = shopOrder.OrderId,
                OrderDate = shopOrder.OrderDate,
                OrderTotal = shopOrder.OrderTotal,
                ShippingCost = shopOrder.ShippingCost,
                DiscountMoney = shopOrder.DiscountMoney,
                UserId = shopOrder.UserId,
                User = appUserDto,
                PaymentMethodId = shopOrder.PaymentMethodId,
                PaymentMethod = paymentMethodDto,
                AddressId = shopOrder.AddressId,
                Address = addressDto,
                ShippingMethodId = shopOrder.ShippingMethodId,
                OrderStatusId = shopOrder.OrderStatusId,
                Items = orderItemsDto
            };
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShopOrders(
            [FromQuery] string? search,
            [FromQuery] string? status,
            [FromQuery] string? sort)
        {
            var allShopOrdersQuery = _shopOrderService.FilterAllShopOrders();

            // Search Order
            if (!string.IsNullOrEmpty(search))
            {
                allShopOrdersQuery = _shopOrderService.FilterSearch(allShopOrdersQuery, search);
            }

            // Filter order status
            if (!string.IsNullOrEmpty(status))
            {
                allShopOrdersQuery = _shopOrderService.FilterByStatus(allShopOrdersQuery, status);
            }

            // Sort order
            if (string.IsNullOrEmpty(sort)) sort = "creationtimedesc";

            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower())
                {
                    case "creationtimedesc":
                        allShopOrdersQuery = _shopOrderService.FilterSortByCreationTime(allShopOrdersQuery, isDesc: true);
                        break;
                    case "creationtimeasc":
                        allShopOrdersQuery = _shopOrderService.FilterSortByCreationTime(allShopOrdersQuery, isDesc: false);
                        break;
                    case "totaldesc":
                        allShopOrdersQuery = _shopOrderService.FilterSortByTotal(allShopOrdersQuery, isDesc: true);
                        break;
                    case "totalasc":
                        allShopOrdersQuery = _shopOrderService.FilterSortByTotal(allShopOrdersQuery, isDesc: false);
                        break;
                    default:
                        break;
                }
            }

            // Query to database
            var allShopOrders = await allShopOrdersQuery.ToListAsync();

            List<ShopOrderDTO> orders = new List<ShopOrderDTO>();
            foreach (var item in allShopOrders)
            {
                orders.Add(await ConvertToShopOrderDto(item));
            }

            return Ok(new ResponseDTO()
            {
                Data = orders
            });
        }

        [HttpGet("myorders")]
        public async Task<IActionResult> GetUserShopOrders()
        {
            // Logged user
            var currentIdentityUser = _userService.GetCurrentUser(HttpContext.User);
            if (currentIdentityUser == null)
            {
                return NotFound();
            }

            // Exist user from database
            var currentUser = await _userService.GetUserByUserName(currentIdentityUser.UserName);
            if (currentUser == null)
            {
                return NotFound();
            }

            var userShopOrders = await _shopOrderService.GetUserShopOrders(currentUser.UserId);

            List<ShopOrderDTO> orders = new List<ShopOrderDTO>();
            foreach (var item in userShopOrders)
            {
                orders.Add(await ConvertToShopOrderDto(item));
            }

            return Ok(new ResponseDTO()
            {
                Data = orders
            });
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> GetAShopOrder([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            var shopOrder = await _shopOrderService.GetShopOrder(id.Value);
            if (shopOrder == null)
            {
                return NotFound();
            }

            return Ok(new ResponseDTO()
            {
                Data = await ConvertToShopOrderDto(shopOrder)
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddShopOrder([FromBody] ShopOrderDTO shopOrderDto)
        {
            if (shopOrderDto == null ||
                shopOrderDto.Items == null ||
                shopOrderDto.Items.Count == 0)
            {
                return BadRequest();
            }

            // Logged user
            var currentIdentityUser = _userService.GetCurrentUser(HttpContext.User);
            if (currentIdentityUser == null)
            {
                return NotFound();
            }

            // Exist user from database
            var currentUser = await _userService.GetUserByUserName(currentIdentityUser.UserName);
            if (currentUser == null)
            {
                return NotFound();
            }

            // Get init order status
            OrderStatus initOrderStatus = await _orderStatusService.GetOrderStatus("created");
            if (initOrderStatus == null)
            {
                try
                {
                    var newInitOrderStatus = new OrderStatus()
                    {
                        Name = "Đã tạo đơn hàng",
                        Status = "created"
                    };

                    initOrderStatus = await _orderStatusService.AddOrderStatus(newInitOrderStatus);
                }
                catch (Exception error)
                {
                    return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
                }
            }

            try
            {
                // Calculate Order Total Price
                // Total Items Price
                decimal totalItemsPrice = 0;
                foreach (var item in shopOrderDto.Items)
                {
                    var productItem = await _productService.GetItem(item.ProductItemId);
                    if (productItem != null)
                    {
                        // Get max discount rate from promotions
                        var promotions = await _productService.GetItemPromotions(item.ProductItemId);
                        int maxDiscountRate = 0;
                        if (promotions.Count > 0)
                        {
                            maxDiscountRate = promotions.Max(p => p.DiscountRate);
                        }

                        totalItemsPrice += (productItem.Price - (productItem.Price * maxDiscountRate / 100)) * item.Qty;
                    }
                    else
                    {
                        return NotFound(new ErrorDTO() { Title = "product item not found", Status = 400 });
                    }
                }

                // Shipping Cost
                decimal shippingCost = 0;
                if (shopOrderDto.ShippingMethodId.HasValue)
                {
                    var shippingMethod = await _shippingMethodService.GetShippingMethod(shopOrderDto.ShippingMethodId.Value);
                    if (shippingMethod != null)
                    {
                        shippingCost = shippingMethod.Price;
                    }
                }

                // Discount money
                decimal discountMoney = 0;

                var newShopOrder = new ShopOrder()
                {
                    UserId = currentUser.UserId,
                    PaymentMethodId = shopOrderDto.PaymentMethodId,
                    AddressId = shopOrderDto.AddressId,
                    ShippingMethodId = shopOrderDto.ShippingMethodId,
                    OrderStatusId = initOrderStatus.OrderStatusId,
                    OrderDate = DateTime.UtcNow,
                    ShippingCost = shippingCost,
                    DiscountMoney = discountMoney,
                    OrderTotal = totalItemsPrice + shippingCost - discountMoney,
                };

                // Add Shop Order
                var createdShopOrder = await _shopOrderService.AddShopOrder(newShopOrder);
                if (createdShopOrder == null)
                {
                    return StatusCode(
                                StatusCodes.Status500InternalServerError,
                                new ErrorDTO() { Title = "Can not create shop order", Status = 500 });
                }

                if (createdShopOrder != null)
                {
                    try
                    {
                        foreach (var item in shopOrderDto.Items)
                        {
                            // Get order item price
                            var productItem = await _productService.GetItem(item.ProductItemId) ?? throw new Exception("product item not found");

                            // Get max discount rate from promotions
                            var promotions = await _productService.GetItemPromotions(productItem.ProductItemId);
                            int maxDiscountRate = 0;
                            if (promotions.Count > 0)
                            {
                                maxDiscountRate = promotions.Max(p => p.DiscountRate);
                            }

                            var newOrderItem = new OrderItem()
                            {
                                OrderId = createdShopOrder.OrderId,
                                ProductItemId = item.ProductItemId,
                                Qty = item.Qty,
                                Price = productItem.Price,
                                DiscountRate = maxDiscountRate,
                            };

                            var createdOrderItem = await _shopOrderService.AddOrderItem(newOrderItem);
                            if (createdOrderItem == null)
                            {
                                return StatusCode(
                                    StatusCodes.Status500InternalServerError,
                                    new ErrorDTO() { Title = "Can not create order item", Status = 500 });
                            }
                            else
                            {
                                // Update qty in stock
                                productItem.QtyInStock -= item.Qty;
                                var descreaseResult = await _productService.UpdateProductItem(productItem);
                                if (descreaseResult == null)
                                {
                                    return StatusCode(
                                    StatusCodes.Status500InternalServerError,
                                    new ErrorDTO() { Title = "Can not update qty in stock", Status = 500 });
                                }
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        // Remove created shop order on fail
                        await _shopOrderService.RemoveShopOrder(createdShopOrder);

                        return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
                    }
                }

                return CreatedAtAction(nameof(GetUserShopOrders),
                    new { id = createdShopOrder?.UserId },
                    new ResponseDTO()
                    {
                        Data = await ConvertToShopOrderDto(createdShopOrder)
                    });
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }
        }

        // Cancel order
        [HttpPut("cancel/{id?}")]
        public async Task<IActionResult> CancelShopOrder([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            var existShopOrder = await _shopOrderService.GetShopOrder(id.Value);
            if (existShopOrder == null)
            {
                return NotFound();
            }

            try
            {
                var cancelResult = await _shopOrderService.CancelShopOrder(existShopOrder);
                if (cancelResult == null)
                {
                    return StatusCode(
                                    StatusCodes.Status500InternalServerError,
                                    new ErrorDTO() { Title = "Can not cancel order", Status = 500 });
                }

                return Ok(new ResponseDTO()
                {
                    Data = await ConvertToShopOrderDto(cancelResult)
                });
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }
        }

        // delivery order
        [HttpPut("delivery/{id?}")]
        public async Task<IActionResult> DeliveryShopOrder([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            var existShopOrder = await _shopOrderService.GetShopOrder(id.Value);
            if (existShopOrder == null)
            {
                return NotFound();
            }

            try
            {
                var cancelResult = await _shopOrderService.DeliveryOrder(existShopOrder);
                if (cancelResult == null)
                {
                    return StatusCode(
                                    StatusCodes.Status500InternalServerError,
                                    new ErrorDTO() { Title = "Can not delivery order", Status = 500 });
                }

                return Ok(new ResponseDTO()
                {
                    Data = await ConvertToShopOrderDto(cancelResult)
                });
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }
        }

        [HttpDelete("{id?}")]
        public async Task<IActionResult> RemoveShopOrder([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            var existShopOrder = await _shopOrderService.GetShopOrder(id.Value);
            if (existShopOrder == null)
            {
                return NotFound();
            }

            try
            {
                var removeShopOrderResult = await _shopOrderService.RemoveShopOrder(existShopOrder);
                if (removeShopOrderResult == 0)
                {
                    return StatusCode(
                                StatusCodes.Status500InternalServerError,
                                new ErrorDTO() { Title = "Can not delete shop order", Status = 500 });
                }
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = await ConvertToShopOrderDto(existShopOrder)
            });
        }
    }
}
