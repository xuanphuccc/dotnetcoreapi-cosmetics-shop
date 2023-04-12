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
            var promotions = _promotionService.FilterAllPromotions();

            // Search promotions
            if (!string.IsNullOrEmpty(search))
            {
                promotions = _promotionService.FilterSearch(promotions, search);
            }

            // Filter by status
            if(!string.IsNullOrEmpty(status))
            {
                promotions = _promotionService.FilterByStatus(promotions, status);
            }

            if(!string.IsNullOrEmpty(sort)) {
                switch (sort)
                {
                    case "nameDesc":
                        promotions = _promotionService.FilterSortByName(promotions, isDesc: true);
                        break;
                    case "nameAsc":
                        promotions = _promotionService.FilterSortByName(promotions, isDesc: false);
                        break;
                    case "creationTimeDesc":
                        promotions = _promotionService.FilterSortByCreationTime(promotions, isDesc: true);
                        break;
                    case "creationTimeAsc":
                        promotions = _promotionService.FilterSortByCreationTime(promotions, isDesc: false);
                        break;
                    case "discountDesc":
                        promotions = _promotionService.FilterSortByDiscountRate(promotions, isDesc: true);
                        break;
                    case "discountAsc":
                        promotions = _promotionService.FilterSortByDiscountRate(promotions, isDesc: false);
                        break;
                    default:
                        break;
                }
            }

            return Ok(await promotions.ToListAsync());
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
                return NotFound();
            }

            return Ok(promotion);
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
                return BadRequest(new ErrorDTO() { Title = "Name already exist", Status = 400 });
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
                if (createdPromotion == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                return CreatedAtAction(nameof(GetPromotion), new { id = createdPromotion.PromotionId }, createdPromotion);
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

            // Find existing Promotion with PromotionId = id
            var existPromotion = await _promotionService.GetPromotion(id.Value);
            if (existPromotion == null)
            {
                return NotFound();
            }

            // Check existing Category name
            var isExistPromotionName = await _promotionService.GetExistPromotionName(promotion.Name);
            if (isExistPromotionName == true && promotion.Name != existPromotion.Name)
            {
                return BadRequest(new ErrorDTO() { Title = "Name already exist", Status = 400 });
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

                // Updating Promotion
                var result = await _promotionService.UpdatePromotion(newPromotion);
                if (result == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                return Ok(result);
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }
        }

        // DELETE: /api/promotions/{id}
        [HttpDelete("{id?}")]
        public async Task<IActionResult> RemovePromotion([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            // Find existing Promotion
            var existPromotion = await _promotionService.GetPromotion(id.Value);
            if (existPromotion == null)
            {
                return NotFound();
            }

            try
            {
                // Removing Promotion
                var result = await _promotionService.RemovePromotion(existPromotion);
                if (result == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                return Ok(result);
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }
        }
    }
}
