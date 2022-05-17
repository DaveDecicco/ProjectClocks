using Microsoft.AspNetCore.Mvc; // [Route], [ApiController], ControllerBase
using ProjectClocks.Common.EntityModels.SqlServer; // Client
using ProjectClocks.ClientService.Repositories; // IClientRepository

namespace ProjectClocks.ClientService.Controllers
{
    // base address: api/clients
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository repo;

        // constructor injects repository registered in Program.cs
        public ClientsController(IClientRepository repo)
        {
            this.repo = repo;
        }

        // GET: api/clients
        // this will always return a list of clients (but it might be empty)
        /// <summary>
        /// Gets a list of all clients
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Client>))]
        public async Task<IEnumerable<Client>> GetClients()
        {
            return await repo.RetrieveAllAsync();
        }

        // GET: api/clients/[id]
        /// <summary>
        /// Gets details for a single client
        /// </summary>
        /// <param name="id">Id of the client</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = nameof(GetClient))] // named route
        [ProducesResponseType(200, Type = typeof(Client))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetClient(int id)
        {
            Client? client = await repo.RetrieveAsync(id);
            if (client == null)
            {
                return NotFound(); // 404 Resource not found
            }
            return Ok(client); // 200 OK with customer in body
        }

        // POST: api/clients
        // BODY: Client (JSON, XML)
        /// <summary>
        /// Creates a new client
        /// </summary>
        /// <param name="client">Client information to create</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Client))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] Client client)
        {
            if (client == null)
            {
                return BadRequest(); // 400 Bad request
            }
            Client? addedClient = await repo.CreateAsync(client);
            if (addedClient == null)
            {
                return BadRequest("Repository failed to create client.");
            }
            else
            {
                return CreatedAtRoute( // 201 Created
                routeName: nameof(GetClient),
                routeValues: new { id = addedClient.Id },
                value: addedClient);
            }
        }

        // PUT: api/clients/[id]
        // BODY: Client (JSON, XML)
        /// <summary>
        /// Updates client properties
        /// </summary>
        /// <param name="id">Id of the client to be updated</param>
        /// <param name="client">Client information to be updated</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] Client client)
        {
            if (client == null || client.Id != id)
            {
                return BadRequest(); // 400 Bad request
            }
            Client? existing = await repo.RetrieveAsync(id);
            if (existing == null)
            {
                return NotFound(); // 404 Resource not found
            }
            await repo.UpdateAsync(id, client);
            return new NoContentResult(); // 204 No content
        }

        // DELETE: api/clients/[id]
        /// <summary>
        /// Deletes a client
        /// </summary>
        /// <param name="id">Id of the client to delete</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            Client? existing = await repo.RetrieveAsync(id);
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
                $"Client {id} was found but failed to delete.");
            }
        }
    }
}
