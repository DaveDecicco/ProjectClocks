using Microsoft.AspNetCore.Mvc; // [Route], [ApiController], ControllerBase
using ProjectClocks.Common.EntityModels.SqlServer; // UserProject
using ProjectClocks.UserProjectService.Repositories; // IUserProjectRepository

namespace ProjectClocks.UserProjectService.Controllers
{
    // base address: api/userprojects
    [Route("api/[controller]")]
    [ApiController]
    public class UserProjectsController : ControllerBase
    {
        private readonly IUserProjectRepository repo;

        // constructor injects repository registered in Program.cs
        public UserProjectsController(IUserProjectRepository repo)
        {
            this.repo = repo;
        }

        // GET: api/userprojects
        // this will always return a list of userprojects (but it might be empty)
        /// <summary>
        /// Gets a list of all user projects
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserProject>))]
        public async Task<IEnumerable<UserProject>> GetUserProjects()
        {
            return await repo.RetrieveAllAsync();
        }

        // GET: api/userprojects/[id]
        /// <summary>
        /// Gets details for a single user project
        /// </summary>
        /// <param name="id">Id of the user project</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = nameof(GetUserProject))] // named route
        [ProducesResponseType(200, Type = typeof(UserProject))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserProject(int id)
        {
            UserProject? userProject = await repo.RetrieveAsync(id);
            if (userProject == null)
            {
                return NotFound(); // 404 Resource not found
            }
            return Ok(userProject); // 200 OK with user project in body
        }

        // GET: api/userprojects/group/[id]
        /// <summary>
        /// Gets a list of user projects by group_id
        /// </summary>
        /// <param name="id">Id of the group which user projects belong to</param>
        /// <returns></returns>
        [HttpGet("group/{id}", Name = nameof(GetUserProjectsByGroup))] // named route
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserProject>))]
        public async Task<IEnumerable<UserProject>> GetUserProjectsByGroup(int id)
        {
            return await repo.RetrieveByGroupAsync(id);
        }

        // GET: api/userprojects/user/[id]
        /// <summary>
        /// Gets a list of user projects by user_id
        /// </summary>
        /// <param name="id">Id of the user which user projects belong to</param>
        /// <returns></returns>
        [HttpGet("user/{id}", Name = nameof(GetUserProjectsByUser))] // named route
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserProject>))]
        public async Task<IEnumerable<UserProject>> GetUserProjectsByUser(int id)
        {
            return await repo.RetrieveByUserAsync(id);
        }

        // POST: api/userprojects
        // BODY: UserProject (JSON, XML)
        /// <summary>
        /// Creates a new user project
        /// </summary>
        /// <param name="userProject">User Project information to create</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(UserProject))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] UserProject userProject)
        {
            if (userProject == null)
            {
                return BadRequest(); // 400 Bad request
            }
            UserProject? addedUserProject = await repo.CreateAsync(userProject);
            if (addedUserProject == null)
            {
                return BadRequest("Repository failed to create user project.");
            }
            else
            {
                return CreatedAtRoute( // 201 Created
                routeName: nameof(GetUserProject),
                routeValues: new { id = addedUserProject.Id },
                value: addedUserProject);
            }
        }

        // PUT: api/userprojects/[id]
        // BODY: UserProject (JSON, XML)
        /// <summary>
        /// Updates user project properties
        /// </summary>
        /// <param name="id">Id of the user project to be updated</param>
        /// <param name="userProject">User Project information to be updated</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] UserProject userProject)
        {
            if (userProject == null || userProject.Id != id)
            {
                return BadRequest(); // 400 Bad request
            }
            UserProject? existing = await repo.RetrieveAsync(id);
            if (existing == null)
            {
                return NotFound(); // 404 Resource not found
            }
            await repo.UpdateAsync(id, userProject);
            return new NoContentResult(); // 204 No content
        }

        // DELETE: api/userprojects/[id]
        /// <summary>
        /// Deletes a user project
        /// </summary>
        /// <param name="id">Id of the user project to delete</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            UserProject? existing = await repo.RetrieveAsync(id);
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
                $"User Project {id} was found but failed to delete.");
            }
        }
    }
}
