using Microsoft.AspNetCore.Mvc; // [Route], [ApiController], ControllerBase
using ProjectClocks.Common.EntityModels.SqlServer; // ReportConfiguration
using ProjectClocks.ReportConfigurationService.Repositories; // IReportConfigurationRepository


namespace ProjectClocks.ReportConfigurationService.Controllers
{
    // base address: api/reportconfigurations
    [Route("api/[controller]")]
    [ApiController]
    public class ReportConfigurationsController : ControllerBase
    {
        private readonly IReportConfigurationRepository repo;

        // constructor injects repository registered in Program.cs
        public ReportConfigurationsController(IReportConfigurationRepository repo)
        {
            this.repo = repo;
        }

        // GET: api/reportconfigurations
        // this will always return a list of report configurations (but it might be empty)
        /// <summary>
        /// Gets a list of all report configurations
        /// </summary>
        /// <returns>List of all report configurations</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReportConfiguration>))]
        public async Task<IEnumerable<ReportConfiguration>> GetReportConfigurations()
        {
            return await repo.RetrieveAllAsync();
        }

        // GET: api/reportconfigurations/[id]
        /// <summary>
        /// Gets details for a single report configuration
        /// </summary>
        /// <param name="id">Id of the report configuration</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = nameof(GetReportConfiguration))] // named route
        [ProducesResponseType(200, Type = typeof(ReportConfiguration))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetReportConfiguration(int id)
        {
            ReportConfiguration? reportconfiguration = await repo.RetrieveAsync(id);
            if (reportconfiguration == null)
            {
                return NotFound(); // 404 Resource not found
            }
            return Ok(reportconfiguration); // 200 OK with reportconfiguration in body
        }

        // POST: api/reportconfigurations
        // BODY: ReportConfiguration (JSON, XML)
        /// <summary>
        /// Creates a new report configuration
        /// </summary>
        /// <param name="reportconfiguration">The report configuration to create</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(ReportConfiguration))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] ReportConfiguration reportconfiguration)
        {
            if (reportconfiguration == null)
            {
                return BadRequest(); // 400 Bad request
            }
            ReportConfiguration? addedInvoice = await repo.CreateAsync(reportconfiguration);
            if (addedInvoice == null)
            {
                return BadRequest("Repository failed to create Invoice.");
            }
            else
            {
                return CreatedAtRoute( // 201 Created
                routeName: nameof(GetReportConfiguration),
                routeValues: new { id = addedInvoice.Id },
                value: addedInvoice);
            }
        }

        // PUT: api/reportconfigurations/[id]
        // BODY: ReportConfiguration (JSON, XML)
        /// <summary>
        /// Updates report configuration properties
        /// </summary>
        /// <param name="id">Id of the report configuration to update</param>
        /// <param name="reportconfiguration">The updated report configuration information</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] ReportConfiguration reportconfiguration)
        {
            if (reportconfiguration == null || reportconfiguration.Id != id)
            {
                return BadRequest(); // 400 Bad request
            }
            ReportConfiguration? existing = await repo.RetrieveAsync(id);
            if (existing == null)
            {
                return NotFound(); // 404 Resource not found
            }
            await repo.UpdateAsync(id, reportconfiguration);
            return new NoContentResult(); // 204 No content
        }

        // DELETE: api/reportconfigurations/[id]
        /// <summary>
        /// Deletes an report configuration
        /// </summary>
        /// <param name="id">Id of the report configuration to delete</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            ReportConfiguration? existing = await repo.RetrieveAsync(id);
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
                $"ReportConfiguration {id} was found but failed to delete.");
            }
        }
    }
}
