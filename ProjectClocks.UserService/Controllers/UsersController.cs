using Microsoft.AspNetCore.Mvc; // [Route], [ApiController], ControllerBase
using ProjectClocks.Common.EntityModels.SqlServer; // User
using ProjectClocks.UserService.Repositories; // IUserRepository

namespace ProjectClocks.UserService.Controllers
{
    // base address: api/users
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository repo;

        // constructor injects repository registered in Program.cs
        public UsersController(IUserRepository repo)
        {
            this.repo = repo;
        }

        // GET: api/users
        // this will always return a list of users (but it might be empty)
        /// <summary>
        /// Gets a list of all users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        public async Task<IEnumerable<User>> GetUsers()
        {
            return await repo.RetrieveAllAsync();
        }

        // GET: api/users/[id]
        /// <summary>
        /// Gets details for a single user
        /// </summary>
        /// <param name="id">Id of the user</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = nameof(GetUser))] // named route
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUser(int id)
        {
            User? user = await repo.RetrieveAsync(id);
            if (user == null)
            {
                return NotFound(); // 404 Resource not found
            }
            return Ok(user); // 200 OK with customer in body
        }

        // GET: api/users/group/[id]
        /// <summary>
        /// Gets a list of users by group_id
        /// </summary>
        /// <param name="id">Id of the group to retrieve users which belong</param>
        /// <returns></returns>
        [HttpGet("group/{id}", Name = nameof(GetUsersByGroup))] // named route
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        public async Task<IEnumerable<User>> GetUsersByGroup(int id)
        {
            return await repo.RetrieveByGroupAsync(id);
        }

        // GET: api/users/role/[id]
        /// <summary>
        /// Gets a list of users by role_id
        /// </summary>
        /// <param name="id">Id of the role to retrieve users which belong</param>
        /// <returns></returns>
        [HttpGet("role/{id}", Name = nameof(GetUsersByRole))] // named route
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        public async Task<IEnumerable<User>> GetUsersByRole(int id)
        {
            return await repo.RetrieveByRoleAsync(id);
        }

        // GET: api/users/client/[id]
        /// <summary>
        /// Gets a list of users by client_id
        /// </summary>
        /// <param name="id">Id of the client to retrieve users which belong</param>
        /// <returns></returns>
        [HttpGet("client/{id}", Name = nameof(GetUsersByClient))] // named route
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        public async Task<IEnumerable<User>> GetUsersByClient(int id)
        {
            return await repo.RetrieveByClientAsync(id);
        }

        // POST: api/users
        // BODY: User (JSON, XML)
        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="user">User information to create</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(User))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest(); // 400 Bad request
            }
            User? addedUser = await repo.CreateAsync(user);
            if (addedUser == null)
            {
                return BadRequest("Repository failed to create user.");
            }
            else
            {
                return CreatedAtRoute( // 201 Created
                routeName: nameof(GetUser),
                routeValues: new { id = addedUser.Id },
                value: addedUser);
            }
        }

        // PUT: api/users/[id]
        // BODY: User (JSON, XML)
        /// <summary>
        /// Updates user properties
        /// </summary>
        /// <param name="id">Id of the user to be updated</param>
        /// <param name="user">User information to be updated</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] User user)
        {
            if (user == null || user.Id != id)
            {
                return BadRequest(); // 400 Bad request
            }
            User? existing = await repo.RetrieveAsync(id);
            if (existing == null)
            {
                return NotFound(); // 404 Resource not found
            }
            await repo.UpdateAsync(id, user);
            return new NoContentResult(); // 204 No content
        }

        // DELETE: api/users/[id]
        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="id">Id of the user to delete</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            User? existing = await repo.RetrieveAsync(id);
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
                $"User {id} was found but failed to delete.");
            }
        }
    }
}
