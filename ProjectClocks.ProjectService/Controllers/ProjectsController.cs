using Microsoft.AspNetCore.Mvc; // [Route], [ApiController], ControllerBase
using ProjectClocks.Common.EntityModels.SqlServer; // Project
using ProjectClocks.ProjectService.Repositories; // IProjectRepository

namespace ProjectClocks.ProjectService.Controllers
{
    // base address: api/projects
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectRepository repo;

        // constructor injects repository registered in Program.cs
        public ProjectsController(IProjectRepository repo)
        {
            this.repo = repo;
        }

        // GET: api/projects
        // this will always return a list of projects (but it might be empty)
        /// <summary>
        /// Gets a list of all projects
        /// </summary>
        /// <returns>List of all projects</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Project>))]
        public async Task<IEnumerable<Project>> GetProjects()
        {
            return await repo.RetrieveAllAsync();
        }

        // GET: api/projects/[id]
        /// <summary>
        /// Gets details for a single project
        /// </summary>
        /// <param name="id">Id of the project</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = nameof(GetProject))] // named route
        [ProducesResponseType(200, Type = typeof(Project))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProject(int id)
        {
            Project? project = await repo.RetrieveAsync(id);
            if (project == null)
            {
                return NotFound(); // 404 Resource not found
            }
            return Ok(project); // 200 OK with project in body
        }

        // POST: api/projects
        // BODY: Project (JSON, XML)
        /// <summary>
        /// Creates a new project
        /// </summary>
        /// <param name="project">The project to create</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Project))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] Project project)
        {
            if (project == null)
            {
                return BadRequest(); // 400 Bad request
            }
            Project? addedInvoice = await repo.CreateAsync(project);
            if (addedInvoice == null)
            {
                return BadRequest("Repository failed to create Invoice.");
            }
            else
            {
                return CreatedAtRoute( // 201 Created
                routeName: nameof(GetProject),
                routeValues: new { id = addedInvoice.Id },
                value: addedInvoice);
            }
        }

        // PUT: api/projects/[id]
        // BODY: Project (JSON, XML)
        /// <summary>
        /// Updates project properties
        /// </summary>
        /// <param name="id">Id of the project to update</param>
        /// <param name="project">The updated project information</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] Project project)
        {
            if (project == null || project.Id != id)
            {
                return BadRequest(); // 400 Bad request
            }
            Project? existing = await repo.RetrieveAsync(id);
            if (existing == null)
            {
                return NotFound(); // 404 Resource not found
            }
            await repo.UpdateAsync(id, project);
            return new NoContentResult(); // 204 No content
        }

        // DELETE: api/projects/[id]
        /// <summary>
        /// Deletes an project
        /// </summary>
        /// <param name="id">Id of the project to delete</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            Project? existing = await repo.RetrieveAsync(id);
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
                $"Project {id} was found but failed to delete.");
            }
        }
    }
}
