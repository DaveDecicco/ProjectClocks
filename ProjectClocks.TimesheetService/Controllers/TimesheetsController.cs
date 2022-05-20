using Microsoft.AspNetCore.Mvc; // [Route], [ApiController], ControllerBase
using ProjectClocks.Common.EntityModels.SqlServer; // Timesheet
using ProjectClocks.TimesheetService.Repositories; // ITimeSheetRepository

namespace ProjectClocks.TimesheetService.Controllers
{
    // base address: api/timesheets
    [Route("api/[controller]")]
    [ApiController]
    public class TimesheetsController : ControllerBase
    {
        private readonly ITimeSheetRepository repo;

        // constructor injects repository registered in Program.cs
        public TimesheetsController(ITimeSheetRepository repo)
        {
            this.repo = repo;
        }

        // GET: api/timesheets
        // this will always return a list of timesheets (but it might be empty)
        /// <summary>
        /// Gets a list of all timesheets
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<TimeSheet>))]
        public async Task<IEnumerable<TimeSheet>> GetTimeSheets()
        {
            return await repo.RetrieveAllAsync();
        }

        // GET: api/timesheets/[id]
        /// <summary>
        /// Gets details for a single timesheet
        /// </summary>
        /// <param name="id">Id of the timesheet</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = nameof(GetTimeSheet))] // named route
        [ProducesResponseType(200, Type = typeof(TimeSheet))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTimeSheet(int id)
        {
            TimeSheet? timeSheet = await repo.RetrieveAsync(id);
            if (timeSheet == null)
            {
                return NotFound(); // 404 Resource not found
            }
            return Ok(timeSheet); // 200 OK with timesheet in body
        }

        // GET: api/timesheets/group/[id]
        /// <summary>
        /// Gets a list of timesheets by group_id
        /// </summary>
        /// <param name="id">Id of the group to which timesheets belong</param>
        /// <returns></returns>
        [HttpGet("group/{id}", Name = nameof(GetTimeSheetsByGroup))] // named route
        [ProducesResponseType(200, Type = typeof(IEnumerable<TimeSheet>))]
        public async Task<IEnumerable<TimeSheet>> GetTimeSheetsByGroup(int id)
        {
            return await repo.RetrieveByGroupAsync(id);
        }

        // GET: api/timesheets/user/[id]
        /// <summary>
        /// Gets a list of timesheets by user_id
        /// </summary>
        /// <param name="id">Id of the user which timesheets belong</param>
        /// <returns></returns>
        [HttpGet("user/{id}", Name = nameof(GetTimeSheetsByUser))] // named route
        [ProducesResponseType(200, Type = typeof(IEnumerable<TimeSheet>))]
        public async Task<IEnumerable<TimeSheet>> GetTimeSheetsByUser(int id)
        {
            return await repo.RetrieveByUserAsync(id);
        }

        // GET: api/timesheets/client/[id]
        /// <summary>
        /// Gets a list of timesheets by client_id
        /// </summary>
        /// <param name="id">Id of the client which timesheets belong</param>
        /// <returns></returns>
        [HttpGet("client/{id}", Name = nameof(GetTimeSheetsByClient))] // named route
        [ProducesResponseType(200, Type = typeof(IEnumerable<TimeSheet>))]
        public async Task<IEnumerable<TimeSheet>> GetTimeSheetsByClient(int id)
        {
            return await repo.RetrieveByClientAsync(id);
        }

        // GET: api/timesheets/project/[id]
        /// <summary>
        /// Gets a list of timesheets by project_id
        /// </summary>
        /// <param name="id">Id of the project which timesheets belong</param>
        /// <returns></returns>
        [HttpGet("project/{id}", Name = nameof(GetTimeSheetsByProject))] // named route
        [ProducesResponseType(200, Type = typeof(IEnumerable<TimeSheet>))]
        public async Task<IEnumerable<TimeSheet>> GetTimeSheetsByProject(int id)
        {
            return await repo.RetrieveByProjectAsync(id);
        }

        // POST: api/timesheets
        // BODY: TimeSheet (JSON, XML)
        /// <summary>
        /// Creates a timesheet
        /// </summary>
        /// <param name="timeSheet">Timesheet information to create</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(TimeSheet))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] TimeSheet timeSheet)
        {
            if (timeSheet == null)
            {
                return BadRequest(); // 400 Bad request
            }
            TimeSheet? addedTimeSheet = await repo.CreateAsync(timeSheet);
            if (addedTimeSheet == null)
            {
                return BadRequest("Repository failed to create user time entry.");
            }
            else
            {
                return CreatedAtRoute( // 201 Created
                routeName: nameof(timeSheet),
                routeValues: new { id = addedTimeSheet.Id },
                value: addedTimeSheet);
            }
        }

        // PUT: api/timesheets/[id]
        // BODY: TimeSheet (JSON, XML)
        /// <summary>
        /// Updates timesheet properties
        /// </summary>
        /// <param name="id">Id of the timesheet to be updated</param>
        /// <param name="timeSheet">Timesheet information to be updated</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] TimeSheet timeSheet)
        {
            if (timeSheet == null || timeSheet.Id != id)
            {
                return BadRequest(); // 400 Bad request
            }
            TimeSheet? existing = await repo.RetrieveAsync(id);
            if (existing == null)
            {
                return NotFound(); // 404 Resource not found
            }
            await repo.UpdateAsync(id, timeSheet);
            return new NoContentResult(); // 204 No content
        }

        // DELETE: api/timesheets/[id]
        /// <summary>
        /// Deletes a timesheet
        /// </summary>
        /// <param name="id">Id of the timesheet to delete</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            TimeSheet? existing = await repo.RetrieveAsync(id);
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
