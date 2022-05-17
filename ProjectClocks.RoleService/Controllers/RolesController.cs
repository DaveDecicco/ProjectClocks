using Microsoft.AspNetCore.Mvc; // [Route], [ApiController], ControllerBase
using ProjectClocks.Common.EntityModels.SqlServer; // Role
using ProjectClocks.RoleService.Repositories; // IRoleRepository

namespace ProjectClocks.RoleService.Controllers
{
    // base address: api/roles
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleRepository repo;

        // constructor injects repository registered in Program.cs
        public RolesController(IRoleRepository repo)
        {
            this.repo = repo;
        }

        // GET: api/roles
        // this will always return a list of roles (but it might be empty)
        /// <summary>
        /// Gets a list of all roles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Role>))]
        public async Task<IEnumerable<Role>> GetRoles()
        {
            return await repo.RetrieveAllAsync();
        }

        // GET: api/roles/[id]
        /// <summary>
        /// Gets details for a single role
        /// </summary>
        /// <param name="id">Id of the role</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = nameof(GetRole))] // named route
        [ProducesResponseType(200, Type = typeof(Role))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetRole(int id)
        {
            Role? role = await repo.RetrieveAsync(id);
            if (role == null)
            {
                return NotFound(); // 404 Resource not found
            }
            return Ok(role); // 200 OK with customer in body
        }

        // POST: api/roles
        // BODY: Role (JSON, XML)
        /// <summary>
        /// Creates a new role
        /// </summary>
        /// <param name="role">Role information to create</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Role))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] Role role)
        {
            if (role == null)
            {
                return BadRequest(); // 400 Bad request
            }
            Role? addedRole = await repo.CreateAsync(role);
            if (addedRole == null)
            {
                return BadRequest("Repository failed to create role.");
            }
            else
            {
                return CreatedAtRoute( // 201 Created
                routeName: nameof(GetRole),
                routeValues: new { id = addedRole.Id },
                value: addedRole);
            }
        }

        // PUT: api/roles/[id]
        // BODY: Role (JSON, XML)
        /// <summary>
        /// Updates role properties
        /// </summary>
        /// <param name="id">Id of the role to be updated</param>
        /// <param name="role">Role information to be updated</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] Role role)
        {
            if (role == null || role.Id != id)
            {
                return BadRequest(); // 400 Bad request
            }
            Role? existing = await repo.RetrieveAsync(id);
            if (existing == null)
            {
                return NotFound(); // 404 Resource not found
            }
            await repo.UpdateAsync(id, role);
            return new NoContentResult(); // 204 No content
        }

        // DELETE: api/roles/[id]
        /// <summary>
        /// Deletes a role
        /// </summary>
        /// <param name="id">Id of the role to delete</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            Role? existing = await repo.RetrieveAsync(id);
            if (existing == null)
            {
                return NotFound(); // 404 Resource not found
            }
            bool? deleted = await repo.DeleteAsync(id);
            if (deleted.HasValue && deleted.Value) // short circuit logical AND
            {
                return new NoContentResult(); // 204 No content
            }
            else
            {
                return BadRequest( // 400 Bad request
                $"Role {id} was found but failed to delete.");
            }
        }
    }
}
