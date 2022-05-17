using Microsoft.AspNetCore.Mvc; // [Route], [ApiController], ControllerBase
//using ProjectClocks.Common.EntityModels.SqlServer; // ProjectClocksContext
using ProjectClocks.TaskService.Repositories; // ITaskRepository
using Task = System.Threading.Tasks.Task;
using PCTask = ProjectClocks.Common.EntityModels.SqlServer.Task; // Task

namespace ProjectClocks.TaskService.Controllers
{
    // base address: api/tasks
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskRepository repo;

        // constructor injects repository registered in Program.cs
        public TasksController(ITaskRepository repo)
        {
            this.repo = repo;
        }

        // GET: api/tasks
        // this will always return a list of tasks (but it might be empty)
        /// <summary>
        /// Gets a list of all tasks
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<PCTask>))]
        public async Task<IEnumerable<PCTask>> GetTasks()
        {
            return await repo.RetrieveAllAsync();
        }

        // GET: api/tasks/[id]
        /// <summary>
        /// Gets details for a single task
        /// </summary>
        /// <param name="id">Id of the task</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = nameof(GetTask))] // named route
        [ProducesResponseType(200, Type = typeof(PCTask))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTask(int id)
        {
            PCTask? task = await repo.RetrieveAsync(id);
            if (task == null)
            {
                return NotFound(); // 404 Resource not found
            }
            return Ok(task); // 200 OK with customer in body
        }

        // POST: api/tasks
        // BODY: Task (JSON, XML)
        /// <summary>
        /// Creates a new task
        /// </summary>
        /// <param name="task">Task information to create</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(PCTask))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] PCTask task)
        {
            if (task == null)
            {
                return BadRequest(); // 400 Bad request
            }
            PCTask? addedTask = await repo.CreateAsync(task);
            if (addedTask == null)
            {
                return BadRequest("Repository failed to create task.");
            }
            else
            {
                return CreatedAtRoute( // 201 Created
                routeName: nameof(GetTask),
                routeValues: new { id = addedTask.Id },
                value: addedTask);
            }
        }

        // PUT: api/tasks/[id]
        // BODY: Task (JSON, XML)
        /// <summary>
        /// Updates task properties
        /// </summary>
        /// <param name="id">Id of the task to be updated</param>
        /// <param name="task">Task information to be updated</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] PCTask task)
        {
            if (task == null || task.Id != id)
            {
                return BadRequest(); // 400 Bad request
            }
            PCTask? existing = await repo.RetrieveAsync(id);
            if (existing == null)
            {
                return NotFound(); // 404 Resource not found
            }
            await repo.UpdateAsync(id, task);
            return new NoContentResult(); // 204 No content
        }

        // DELETE: api/tasks/[id]
        /// <summary>
        /// Deletes a task
        /// </summary>
        /// <param name="id">Id of the task to delete</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            PCTask? existing = await repo.RetrieveAsync(id);
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
                $"Task {id} was found but failed to delete.");
            }
        }
    }
}
