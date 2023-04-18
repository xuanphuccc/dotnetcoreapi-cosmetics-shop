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
            [FromQuery] string? sort)
        {

            var providersQuery = _providerService.FilterAllProviders();

            if (!string.IsNullOrEmpty(search))
            {
                providersQuery = _providerService.FilterSearch(providersQuery, search);
            }

            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower())
                {
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

            List<ProviderDTO> providersDto = new List<ProviderDTO>();
            foreach (var provider in providers)
            {
                providersDto.Add(ConvertToProviderDto(provider));
            }

            return Ok(new ResponseDTO()
            {
                Data = providersDto
            });
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> GetProvider([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            // Get provider
            var provider = await _providerService.GetProvider(id.Value);
            if (provider == null)
            {
                return NotFound(new ErrorDTO() { Title = "provider not found", Status = 404 });
            }

            return Ok(new ResponseDTO()
            {
                Data = ConvertToProviderDto(provider)
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddProvider([FromBody] ProviderDTO providerDto)
        {
            if (providerDto == null)
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

                return CreatedAtAction(
                    nameof(GetProvider),
                    new { id = createdProvider.ProviderId },
                    new ResponseDTO()
                    {
                        Data = ConvertToProviderDto(createdProvider),
                        Status = 201,
                    });
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }
        }

        [HttpPut("{id?}")]
        public async Task<IActionResult> UpdateProvider([FromRoute] int? id, [FromBody] ProviderDTO providerDto)
        {
            if (!id.HasValue || providerDto == null)
            {
                return BadRequest();
            }

            // Get exist provider
            var existProvider = await _providerService.GetProvider(id.Value);
            if (existProvider == null)
            {
                return NotFound(new ErrorDTO() { Title = "provider not found", Status = 404 });
            }

            try
            {
                var newProvider = new Provider()
                {
                    ProviderId = existProvider.ProviderId,
                    Name = providerDto.Name
                };

                // Update provider
                if (existProvider.Name != newProvider.Name)
                {
                    var updateResult = await _providerService.UpdateProvider(newProvider);

                    return Ok(new ResponseDTO()
                    {
                        Data = ConvertToProviderDto(updateResult)
                    });
                }
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = ConvertToProviderDto(existProvider),
                Status = 304,
                Title = "not modified",
            });
        }

        [HttpDelete("{id?}")]
        public async Task<IActionResult> RemoveProvider([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            // Get exist provider
            var existProvider = await _providerService.GetProvider(id.Value);
            if (existProvider == null)
            {
                return NotFound(new ErrorDTO() { Title = "provider not found", Status = 404 });
            }

            try
            {
                await _providerService.RemoveProvider(existProvider);
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = ConvertToProviderDto(existProvider)
            });
        }
    }
}
