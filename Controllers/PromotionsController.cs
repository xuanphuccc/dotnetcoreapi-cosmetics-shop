using web_api_cosmetics_shop.Data;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.PromotionService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace web_api_cosmetics_shop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionsController : ControllerBase
    {
        private readonly IPromotionService _promotionService;
        public PromotionsController(IPromotionService promotionData)
        {
            _promotionService = promotionData;
        }

        // GET: /api/promotions
        [HttpGet]
        public async Task<IActionResult> GetPromotions(
            [FromQuery] string? search,
            [FromQuery] string? sort,
            [FromQuery] string? status)
        {
            var promotionsQuery = _promotionService.FilterAllPromotions();

            // Search promotions
            if (!string.IsNullOrEmpty(search))
            {
                promotionsQuery = _promotionService.FilterSearch(promotionsQuery, search);
            }

            // Filter by status
            if (!string.IsNullOrEmpty(status))
            {
                promotionsQuery = _promotionService.FilterByStatus(promotionsQuery, status);
            }

            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower())
                {
                    case "namedesc":
                        promotionsQuery = _promotionService.FilterSortByName(promotionsQuery, isDesc: true);
                        break;
                    case "nameasc":
                        promotionsQuery = _promotionService.FilterSortByName(promotionsQuery, isDesc: false);
                        break;
                    case "creationtimedesc":
                        promotionsQuery = _promotionService.FilterSortByCreationTime(promotionsQuery, isDesc: true);
                        break;
                    case "creationtimeasc":
                        promotionsQuery = _promotionService.FilterSortByCreationTime(promotionsQuery, isDesc: false);
                        break;
                    case "discountdesc":
                        promotionsQuery = _promotionService.FilterSortByDiscountRate(promotionsQuery, isDesc: true);
                        break;
                    case "discountasc":
                        promotionsQuery = _promotionService.FilterSortByDiscountRate(promotionsQuery, isDesc: false);
                        break;
                    default:
                        break;
                }
            }

            var promotions = await promotionsQuery.ToListAsync();

            List<PromotionDTO> promotionsDto = new();
            foreach (var promotion in promotions)
            {
                promotionsDto.Add(_promotionService.ConvertToPromotionDto(promotion));
            }

            return Ok(new ResponseDTO()
            {
                Data = promotionsDto
            });
        }

        // GET: /api/promotions/{id}
        [HttpGet("{id?}")]
        public async Task<IActionResult> GetPromotion([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            // Finding Promotion
            var promotion = await _promotionService.GetPromotion(id.Value);
            if (promotion == null)
            {
                return NotFound(new ErrorDTO() { Title = "promotion not found", Status = 404 });
            }

            return Ok(new ResponseDTO()
            {
                Data = _promotionService.ConvertToPromotionDto(promotion)
            });
        }

        // POST: /api/promotions
        [HttpPost]
        public async Task<IActionResult> CreatePromotion([FromBody] PromotionDTO? promotion)
        {
            if (promotion == null)
            {
                return BadRequest();
            }

            // Check existing Category name
            var isExistPromotionName = await _promotionService.GetExistPromotionName(promotion.Name);
            if (isExistPromotionName == true)
            {
                return BadRequest(new ErrorDTO() { Title = "promotion name already exist", Status = 400 });
            }

            try
            {
                var newPromotion = new Promotion()
                {
                    Name = promotion.Name,
                    Description = promotion.Description,
                    DiscountRate = promotion.DiscountRate,
                    StartDate = promotion.StartDate,
                    EndDate = promotion.EndDate,
                    CreateAt = DateTime.UtcNow,
                };

                // Creating Promotion
                var createdPromotion = await _promotionService.AddPromotion(newPromotion);

                return CreatedAtAction(
                    nameof(GetPromotion),
                    new { id = createdPromotion.PromotionId },
                    new ResponseDTO()
                    {
                        Data = _promotionService.ConvertToPromotionDto(createdPromotion),
                        Status = 201,
                    });
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }
        }

        // PUT: /api/promotions/{id}
        [HttpPut("{id?}")]
        public async Task<IActionResult> UpdatePromotion([FromRoute] int? id, [FromBody] PromotionDTO? promotion)
        {
            if (!id.HasValue || promotion == null)
            {
                return BadRequest();
            }

            // Get exist promotion with
            var existPromotion = await _promotionService.GetPromotion(id.Value);
            if (existPromotion == null)
            {
                return NotFound(new ErrorDTO() { Title = "promotion not found", Status = 404 });
            }

            // Check exist category name
            var isExistPromotionName = await _promotionService.GetExistPromotionName(promotion.Name);
            if (isExistPromotionName == true && promotion.Name != existPromotion.Name)
            {
                return BadRequest(new ErrorDTO() { Title = "promotion name already exist", Status = 400 });
            }

            try
            {
                var newPromotion = new Promotion()
                {
                    PromotionId = existPromotion.PromotionId,
                    Name = promotion.Name,
                    Description = promotion.Description,
                    DiscountRate = promotion.DiscountRate,
                    StartDate = promotion.StartDate,
                    EndDate = promotion.EndDate
                };

                if (existPromotion.Name != newPromotion.Name ||
                    existPromotion.Description != newPromotion.Description ||
                    existPromotion.DiscountRate != newPromotion.DiscountRate ||
                    existPromotion.StartDate != newPromotion.StartDate ||
                    existPromotion.EndDate != newPromotion.EndDate)
                {
                    // Updating Promotion
                    var result = await _promotionService.UpdatePromotion(newPromotion);

                    return Ok(new ResponseDTO()
                    {
                        Data = _promotionService.ConvertToPromotionDto(result)
                    });
                }
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = _promotionService.ConvertToPromotionDto(existPromotion),
                Status = 304,
                Title = "not modified",
            });
        }

        // DELETE: /api/promotions/{id}
        [HttpDelete("{id?}")]
        public async Task<IActionResult> RemovePromotion([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            // Get exist promotion
            var existPromotion = await _promotionService.GetPromotion(id.Value);
            if (existPromotion == null)
            {
                return NotFound(new ErrorDTO() { Title = "promotion not found", Status = 404 });
            }

            try
            {
                // Remove promotion
                await _promotionService.RemovePromotion(existPromotion);
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = _promotionService.ConvertToPromotionDto(existPromotion)
            });
        }
    }
}
