using Microsoft.AspNetCore.Mvc; // [Route], [ApiController], ControllerBase
using ProjectClocks.Common.EntityModels.SqlServer; // Invoice
using ProjectClocks.InvoiceService.Repositories; // IInvoiceRepository

namespace ProjectClocks.InvoiceService.Controllers
{
    // base address: api/invoices
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceRepository repo;

        // constructor injects repository registered in Program.cs
        public InvoicesController(IInvoiceRepository repo)
        {
            this.repo = repo;
        }

        // GET: api/invoices
        // this will always return a list of invoices (but it might be empty)
        /// <summary>
        /// Gets a list of all invoices
        /// </summary>
        /// <returns>List of all invoices</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Invoice>))]
        public async Task<IEnumerable<Invoice>> GetInvoices()
        {
            return await repo.RetrieveAllAsync();
        }

        // GET: api/invoices/[id]
        /// <summary>
        /// Gets details for a single invoice
        /// </summary>
        /// <param name="id">Id of the invoice</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = nameof(GetInvoice))] // named route
        [ProducesResponseType(200, Type = typeof(Invoice))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetInvoice(int id)
        {
            Invoice? invoice = await repo.RetrieveAsync(id);
            if (invoice == null)
            {
                return NotFound(); // 404 Resource not found
            }
            return Ok(invoice); // 200 OK with invoice in body
        }

        // POST: api/invoices
        // BODY: Invoice (JSON, XML)
        /// <summary>
        /// Creates a new invoice
        /// </summary>
        /// <param name="invoice">The invoice to create</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Invoice))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] Invoice invoice)
        {
            if (invoice == null)
            {
                return BadRequest(); // 400 Bad request
            }
            Invoice? addedInvoice = await repo.CreateAsync(invoice);
            if (addedInvoice == null)
            {
                return BadRequest("Repository failed to create Invoice.");
            }
            else
            {
                return CreatedAtRoute( // 201 Created
                routeName: nameof(GetInvoice),
                routeValues: new { id = addedInvoice.Id },
                value: addedInvoice);
            }
        }

        // PUT: api/invoices/[id]
        // BODY: Invoice (JSON, XML)
        /// <summary>
        /// Updates invoice properties
        /// </summary>
        /// <param name="id">Id of the invoice to update</param>
        /// <param name="invoice">The updated invoice information</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] Invoice invoice)
        {
            if (invoice == null || invoice.Id != id)
            {
                return BadRequest(); // 400 Bad request
            }
            Invoice? existing = await repo.RetrieveAsync(id);
            if (existing == null)
            {
                return NotFound(); // 404 Resource not found
            }
            await repo.UpdateAsync(id, invoice);
            return new NoContentResult(); // 204 No content
        }

        // DELETE: api/invoices/[id]
        /// <summary>
        /// Deletes an invoice
        /// </summary>
        /// <param name="id">Id of the invoice to delete</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            Invoice? existing = await repo.RetrieveAsync(id);
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
                $"Invoice {id} was found but failed to delete.");
            }
        }
    }
}
