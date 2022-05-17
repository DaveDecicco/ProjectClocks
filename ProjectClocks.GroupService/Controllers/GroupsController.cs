using Microsoft.AspNetCore.Mvc; // [Route], [ApiController], ControllerBase
using ProjectClocks.Common.EntityModels.SqlServer; // Group
using ProjectClocks.GroupService.Repositories; // IGroupRepository

namespace ProjectClocks.GroupService.Controllers
{
    // base address: api/groups
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupRepository repo;

        // constructor injects repository registered in Program.cs
        public GroupsController(IGroupRepository repo)
        {
            this.repo = repo;
        }

        // GET: api/groups
        // this will always return a list of groups (but it might be empty)
        /// <summary>
        /// Gets a list of all groups
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Group>))]
        public async Task<IEnumerable<Group>> GetGroups()
        {
            return await repo.RetrieveAllAsync();
        }

        // GET: api/groups/[id]
        /// <summary>
        /// Gets details for a single group
        /// </summary>
        /// <param name="id">Id of the group</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = nameof(GetGroup))] // named route
        [ProducesResponseType(200, Type = typeof(Group))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetGroup(int id)
        {
            Group? c = await repo.RetrieveAsync(id);
            if (c == null)
            {
                return NotFound(); // 404 Resource not found
            }
            return Ok(c); // 200 OK with group in body
        }

        // POST: api/groups
        // BODY: Group (JSON, XML)
        /// <summary>
        /// Creates a new group
        /// </summary>
        /// <param name="group">Group information to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Group))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] Group group)
        {
            if (group == null)
            {
                return BadRequest(); // 400 Bad request
            }
            Group? addedGroup = await repo.CreateAsync(group);
            if (addedGroup == null)
            {
                return BadRequest("Repository failed to create Group.");
            }
            else
            {
                return CreatedAtRoute( // 201 Created
                routeName: nameof(GetGroup),
                routeValues: new { id = addedGroup.Id },
                value: addedGroup);
            }
        }

        // PUT: api/groups/[id]
        // BODY: Group (JSON, XML)
        /// <summary>
        /// Updates group properties
        /// </summary>
        /// <param name="id">Id of the group to update</param>
        /// <param name="group">Group information to be updated</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] Group group)
        {
            if (group == null || group.Id != id)
            {
                return BadRequest(); // 400 Bad request
            }
            Group? existing = await repo.RetrieveAsync(id);
            if (existing == null)
            {
                return NotFound(); // 404 Resource not found
            }
            await repo.UpdateAsync(id, group);
            return new NoContentResult(); // 204 No content
        }

        // DELETE: api/groups/[id]
        /// <summary>
        /// Deletes a group
        /// </summary>
        /// <param name="id">Id of the group to be deleted</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            Group? existing = await repo.RetrieveAsync(id);
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
                $"Group {id} was found but failed to delete.");
            }
        }
    }
}
