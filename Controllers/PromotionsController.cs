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
        private readonly IPromotionService _promotionData;
        public PromotionsController(IPromotionService promotionData) { 
            _promotionData = promotionData;
        }

        // GET: /api/promotions
        [HttpGet]
        public async Task<IActionResult> GetPromotions()
        {
            var promotions = await _promotionData.GetPromotionsAsync();
            return Ok(promotions);
        }

        // GET: /api/promotions/{id}
        [HttpGet("{id?}")]
        public async Task<IActionResult> GetPromotion(int? id)
        {
            if (id == null) return BadRequest();

            var promotions = await _promotionData.GetPromotionByIdAsync(id.Value);

            if(promotions == null) return NotFound();

            return Ok(promotions);
        }

        // POST: /api/promotions
        [HttpPost]
        public async Task<IActionResult> CreatePromotion(PromotionDTO? promotion)
        {
            if (promotion == null) return BadRequest();

            //if (!ModelState.IsValid) return BadRequest(ModelState.ErrorCount);

            var newPromotion = new Promotion()
            {
                Name = promotion.Name,
                Description = promotion.Description,
                DiscountRate = promotion.DiscountRate,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate
            };

            var createdPromotion = await _promotionData.AddPromotionAsync(newPromotion);

            if(createdPromotion == null) return BadRequest();

            return CreatedAtAction(nameof(GetPromotion), new { id = createdPromotion.PromotionId }, createdPromotion);
        }

		// PUT: /api/promotions/{id}
		[HttpPut("{id?}")]
        public async Task<IActionResult> UpdatePromotion(int? id, PromotionDTO? promotion)
        {
            if (id == null || promotion == null) return BadRequest();

            // Tìm Promotion đã tồn tại có id
            var existPromotion = await _promotionData.GetPromotionByIdAsync(id.Value);
            if (existPromotion == null) return NotFound();

			var newPromotion = new Promotion()
			{
                PromotionId = existPromotion.PromotionId,
				Name = promotion.Name,
				Description = promotion.Description,
				DiscountRate = promotion.DiscountRate,
				StartDate = promotion.StartDate,
				EndDate = promotion.EndDate
			};

            var result = await _promotionData.UpdatePromotionAsync(newPromotion);

            if (result == null) return BadRequest();

            return Ok(result);
        }

		// DELETE: /api/promotions/{id}
		[HttpDelete("{id?}")]
        public async Task<IActionResult> RemovePromotion(int? id)
        {
            if (id == null) return BadRequest();

            var existPromotion = await _promotionData.GetPromotionByIdAsync(id.Value);

            if (existPromotion == null) return NotFound();

            var result = await _promotionData.RemovePromotionAsync(existPromotion);

            if (result == null) return BadRequest();

            return Ok(result);
        }


    }
}
