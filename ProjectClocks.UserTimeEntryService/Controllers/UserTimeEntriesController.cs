using Microsoft.AspNetCore.Mvc; // [Route], [ApiController], ControllerBase
using ProjectClocks.Common.EntityModels.SqlServer; // UserTimeEntry
using ProjectClocks.UserTimeEntryService.Repositories; // IUserTimeEntryRepository

namespace ProjectClocks.UserTimeEntryService.Controllers
{
    // base address: api/usertimeentries
    [Route("api/[controller]")]
    [ApiController]
    public class UserTimeEntriesController : ControllerBase
    {
        private readonly IUserTimeEntryRepository repo;

        // constructor injects repository registered in Program.cs
        public UserTimeEntriesController(IUserTimeEntryRepository repo)
        {
            this.repo = repo;
        }

        // GET: api/usertimeentries
        // this will always return a list of user time entries (but it might be empty)
        /// <summary>
        /// Gets a list of all user time entries
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserTimeEntry>))]
        public async Task<IEnumerable<UserTimeEntry>> GetUserTimeEntries()
        {
            return await repo.RetrieveAllAsync();
        }

        // GET: api/usertimeentries/[id]
        /// <summary>
        /// Gets details for a single user time entry
        /// </summary>
        /// <param name="id">Id of the user time entry</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = nameof(GetUserTimeEntry))] // named route
        [ProducesResponseType(200, Type = typeof(UserTimeEntry))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserTimeEntry(long id)
        {
            UserTimeEntry? user = await repo.RetrieveAsync(id);
            if (user == null)
            {
                return NotFound(); // 404 Resource not found
            }
            return Ok(user); // 200 OK with usertimeentry in body
        }

        // GET: api/usertimeentries/group/[id]
        /// <summary>
        /// Gets a list of user time entries by group_id
        /// </summary>
        /// <param name="id">Id of the group to which user time entries belong</param>
        /// <returns></returns>
        [HttpGet("group/{id}", Name = nameof(GetUserTimeEntriesByGroup))] // named route
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserTimeEntry>))]
        public async Task<IEnumerable<UserTimeEntry>> GetUserTimeEntriesByGroup(int id)
        {
            return await repo.RetrieveByGroupAsync(id);
        }

        // GET: api/usertimeentries/user/[id]
        /// <summary>
        /// Gets a list of user time entries by user_id
        /// </summary>
        /// <param name="id">Id of the user which user time entries belong</param>
        /// <returns></returns>
        [HttpGet("user/{id}", Name = nameof(GetUserTimeEntriesByUser))] // named route
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserTimeEntry>))]
        public async Task<IEnumerable<UserTimeEntry>> GetUserTimeEntriesByUser(int id)
        {
            return await repo.RetrieveByUserAsync(id);
        }

        // GET: api/usertimeentries/client/[id]
        /// <summary>
        /// Gets a list of user time entries by client_id
        /// </summary>
        /// <param name="id">Id of the client which user time entries belong</param>
        /// <returns></returns>
        [HttpGet("client/{id}", Name = nameof(GetUserTimeEntriesByClient))] // named route
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserTimeEntry>))]
        public async Task<IEnumerable<UserTimeEntry>> GetUserTimeEntriesByClient(int id)
        {
            return await repo.RetrieveByClientAsync(id);
        }

        // GET: api/usertimeentries/project/[id]
        /// <summary>
        /// Gets a list of user time entries by project_id
        /// </summary>
        /// <param name="id">Id of the project which user time entries belong</param>
        /// <returns></returns>
        [HttpGet("project/{id}", Name = nameof(GetUserTimeEntriesByProject))] // named route
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserTimeEntry>))]
        public async Task<IEnumerable<UserTimeEntry>> GetUserTimeEntriesByProject(int id)
        {
            return await repo.RetrieveByProjectAsync(id);
        }

        // GET: api/usertimeentries/task/[id]
        /// <summary>
        /// Gets a list of user time entries by task_id
        /// </summary>
        /// <param name="id">Id of the task which user time entries belong</param>
        /// <returns></returns>
        [HttpGet("task/{id}", Name = nameof(GetUserTimeEntriesByTask))] // named route
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserTimeEntry>))]
        public async Task<IEnumerable<UserTimeEntry>> GetUserTimeEntriesByTask(int id)
        {
            return await repo.RetrieveByTaskAsync(id);
        }

        // POST: api/usertimeentries
        // BODY: UserTimeEntry (JSON, XML)
        /// <summary>
        /// Creates a new user time entry
        /// </summary>
        /// <param name="userTimeEntry">User Time Entry information to create</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(UserTimeEntry))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] UserTimeEntry userTimeEntry)
        {
            if (userTimeEntry == null)
            {
                return BadRequest(); // 400 Bad request
            }
            UserTimeEntry? addedUserTimeEntry = await repo.CreateAsync(userTimeEntry);
            if (addedUserTimeEntry == null)
            {
                return BadRequest("Repository failed to create user time entry.");
            }
            else
            {
                return CreatedAtRoute( // 201 Created
                routeName: nameof(GetUserTimeEntry),
                routeValues: new { id = addedUserTimeEntry.Id },
                value: addedUserTimeEntry);
            }
        }

        // PUT: api/usertimeentries/[id]
        // BODY: UserTimeEntry (JSON, XML)
        /// <summary>
        /// Updates user time entry properties
        /// </summary>
        /// <param name="id">Id of the user time entry to be updated</param>
        /// <param name="userTimeEntry">User Time Entry information to be updated</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] UserTimeEntry userTimeEntry)
        {
            if (userTimeEntry == null || userTimeEntry.Id != id)
            {
                return BadRequest(); // 400 Bad request
            }
            UserTimeEntry? existing = await repo.RetrieveAsync(id);
            if (existing == null)
            {
                return NotFound(); // 404 Resource not found
            }
            await repo.UpdateAsync(id, userTimeEntry);
            return new NoContentResult(); // 204 No content
        }

        // DELETE: api/usertimeentries/[id]
        /// <summary>
        /// Deletes a user time entry
        /// </summary>
        /// <param name="id">Id of the user time entry to delete</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            UserTimeEntry? existing = await repo.RetrieveAsync(id);
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
                $"User Time Entry {id} was found but failed to delete.");
            }
        }
    }
}
