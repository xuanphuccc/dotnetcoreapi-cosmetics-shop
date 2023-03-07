using web_api_cosmetics_shop.Data;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.PromotionService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace web_api_cosmetics_shop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionsController : ControllerBase
    {
        private readonly IPromotionService _promotionService;
        public PromotionsController(IPromotionService promotionData) { 
            _promotionService = promotionData;
        }

        // GET: /api/promotions
        [HttpGet]
        public async Task<IActionResult> GetPromotions()
        {
            var promotions = await _promotionService.GetPromotionsAsync();
            return Ok(promotions);
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
            var promotion = await _promotionService.GetPromotionByIdAsync(id.Value);
            if(promotion == null)
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

			var newPromotion = new Promotion()
            {
                Name = promotion.Name,
                Description = promotion.Description,
                DiscountRate = promotion.DiscountRate,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate
            };

            // Creating Promotion
            var createdPromotion = await _promotionService.AddPromotionAsync(newPromotion);
            if(createdPromotion == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return CreatedAtAction(nameof(GetPromotion), new { id = createdPromotion.PromotionId }, createdPromotion);
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
            var existPromotion = await _promotionService.GetPromotionByIdAsync(id.Value);
            if (existPromotion == null)
            {
				return NotFound();
			}

			// Check existing Category name
			var isExistPromotionName = await _promotionService.GetExistPromotionName(promotion.Name);
			if (isExistPromotionName == true)
			{
				return BadRequest(new ErrorDTO() { Title = "Name already exist", Status = 400 });
			}

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
            var result = await _promotionService.UpdatePromotionAsync(newPromotion);
            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok(result);
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
            var existPromotion = await _promotionService.GetPromotionByIdAsync(id.Value);
            if (existPromotion == null)
            {
                return NotFound();
            }

            // Removing Promotion
            var result = await _promotionService.RemovePromotionAsync(existPromotion);
            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok(result);
        }
    }
}
