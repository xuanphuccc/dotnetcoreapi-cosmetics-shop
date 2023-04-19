using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.RoleService;

namespace web_api_cosmetics_shop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleService.GetAllRoles();

            List<RoleDTO> rolesDto = new();
            foreach (var role in roles)
            {
                rolesDto.Add(_roleService.ConvertToRoleDto(role));
            }

            return Ok(new ResponseDTO()
            {
                Data = rolesDto
            });
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> GetRole(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            var role = await _roleService.GetRoleById(id.Value);
            if (role == null)
            {
                return NotFound(new ErrorDTO() { Title = "role not found", Status = 404 });
            }

            return Ok(new ResponseDTO()
            {
                Data = _roleService.ConvertToRoleDto(role)
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(RoleDTO roleDto)
        {
            if (roleDto == null)
            {
                return BadRequest();
            }

            var existNameRole = await _roleService.GetRoleByName(roleDto.Name);
            if (existNameRole != null)
            {
                return BadRequest(new ErrorDTO() { Title = "name already exist", Status = 400 });
            }

            try
            {
                var newRole = new Role()
                {
                    Name = roleDto.Name
                };

                var createdRole = await _roleService.AddRole(newRole);

                return CreatedAtAction(
                    nameof(GetRole),
                    new { id = createdRole.RoleId },
                    new ResponseDTO()
                    {
                        Data = _roleService.ConvertToRoleDto(createdRole),
                        Status = 201
                    });
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }
        }

        [HttpPut("{id?}")]
        public async Task<IActionResult> UpdateRole(int? id, RoleDTO roleDto)
        {
            if (!id.HasValue || roleDto == null)
            {
                return BadRequest();
            }

            // Get exist role
            var existRole = await _roleService.GetRoleById(id.Value);
            if (existRole == null)
            {
                return NotFound(new ErrorDTO() { Title = "role not found", Status = 404 });
            }

            // Check exist role name
            var existNameRole = await _roleService.GetRoleByName(roleDto.Name);
            if (existNameRole != null && existRole.Name != roleDto.Name)
            {
                return BadRequest(new ErrorDTO() { Title = "name already exist", Status = 400 });
            }

            try
            {
                var updateRole = new Role()
                {
                    RoleId = existRole.RoleId,
                    Name = roleDto.Name
                };

                if (existRole.Name != updateRole.Name)
                {
                    var updatedRole = await _roleService.UpdateRole(updateRole);

                    return Ok(new ResponseDTO()
                    {
                        Data = _roleService.ConvertToRoleDto(updatedRole)
                    });
                }

            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = _roleService.ConvertToRoleDto(existRole),
                Status = 304,
                Title = "not modified",
            });
        }

        [HttpDelete("{id?}")]
        public async Task<IActionResult> DeleteRole(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            var existRole = await _roleService.GetRoleById(id.Value);
            if (existRole == null)
            {
                return NotFound(new ErrorDTO() { Title = "role not found", Status = 404 });
            }

            try
            {
                await _roleService.RemoveRole(existRole);
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = _roleService.ConvertToRoleDto(existRole)
            });
        }
    }
}
