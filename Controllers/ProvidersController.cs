using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.EntityFrameworkCore;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.ProviderService;

namespace web_api_cosmetics_shop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvidersController : ControllerBase
    {
        private readonly IProviderService _providerService;
        public ProvidersController(IProviderService providerService)
        {
            _providerService = providerService;
        }

        [NonAction]
        private ProviderDTO ConvertToProviderDto(Provider provider)
        {
            return new ProviderDTO()
            {
                ProviderId = provider.ProviderId,
                Name = provider.Name,
                CreateAt = provider.CreateAt,
            };
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProviders(
            [FromQuery] string? search,
            [FromQuery] string? sort) {

            var providersQuery = _providerService.FilterAllProviders();

            if(!string.IsNullOrEmpty(search))
            {
                providersQuery = _providerService.FilterSearch(providersQuery, search);
            }

            if(!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower()) {
                    case "creationtimedesc":
                        providersQuery = _providerService.FilterSortByCreationTime(providersQuery, isDesc: true);
                        break;
                    case "creationtimeasc":
                        providersQuery = _providerService.FilterSortByCreationTime(providersQuery, isDesc: false);
                        break;
                    case "namedesc":
                        providersQuery = _providerService.FilterSortByName(providersQuery, isDesc: true);
                        break;
                    case "nameasc":
                        providersQuery = _providerService.FilterSortByName(providersQuery, isDesc: false);
                        break;
                    default:
                        break;
                }
            }


            var providers = await providersQuery.ToListAsync();

            List<ProviderDTO> providerDtos = new List<ProviderDTO>();
            foreach (var provider in providers)
            {
                providerDtos.Add(ConvertToProviderDto(provider));
            }

            return Ok(providerDtos);
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> GetProvider([FromRoute] int? id)
        {
            if(!id.HasValue)
            {
                return BadRequest();
            }

            var provider = await _providerService.GetProvider(id.Value);
            if(provider == null)
            {
                return NotFound();
            }

            return Ok(ConvertToProviderDto(provider));
        }

        [HttpPost]
        public async Task<IActionResult> AddProvider([FromBody] ProviderDTO providerDto)
        {
            if(providerDto == null)
            {
                return BadRequest();
            }

            try
            {
                var newProvider = new Provider()
                {
                    Name = providerDto.Name,
                    CreateAt = DateTime.UtcNow,
                };

                var createdProvider = await _providerService.AddProvider(newProvider);
                if(createdProvider == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                                                new ErrorDTO() { Title = "Can not create provider", Status = 500 });
                }

                return CreatedAtAction(
                    nameof(GetProvider),
                    new { id = createdProvider.ProviderId },
                    ConvertToProviderDto(createdProvider));
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }
        }

        [HttpPut("{id?}")]
        public async Task<IActionResult> UpdateProvider([FromRoute] int? id, [FromBody] ProviderDTO providerDto)
        {
            if(!id.HasValue || providerDto == null)
            {
                return BadRequest();
            }

            var existProvider = await _providerService.GetProvider(id.Value);
            if(existProvider == null)
            {
                return NotFound();
            }

            try
            {
                var newProvider = new Provider()
                {
                    ProviderId = existProvider.ProviderId,
                    Name = providerDto.Name
                };

                if(existProvider.Name != newProvider.Name)
                {
                    var updateResult = await _providerService.UpdateProvider(newProvider);
                    if(updateResult == null) {
                        return StatusCode(StatusCodes.Status500InternalServerError,
                                                    new ErrorDTO() { Title = "Can not update provider", Status = 500 });
                    }
                }

                return Ok(providerDto);
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }
        }

        [HttpDelete("{id?}")]
        public async Task<IActionResult> RemoveProvider([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            var existProvider = await _providerService.GetProvider(id.Value);
            if(existProvider == null)
            {
                return NotFound();
            }

            try
            {
                var removeResult = await _providerService.RemoveProvider(existProvider);
                if (removeResult == 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                                                    new ErrorDTO() { Title = "Can not remove provider", Status = 500 });
                }
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(ConvertToProviderDto(existProvider));
        }
    }
}
