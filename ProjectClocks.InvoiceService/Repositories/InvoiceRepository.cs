using Microsoft.EntityFrameworkCore.ChangeTracking; // EntityEntry<T>
using ProjectClocks.Common.EntityModels.SqlServer; // Invoice
using System.Collections.Concurrent; // ConcurrentDictionary
using Task = System.Threading.Tasks.Task;

namespace ProjectClocks.InvoiceService.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        // use a static thread-safe dictionary field to cache the Invoices
        // TODO: use Redis instead?
        private static ConcurrentDictionary<int, Invoice>? invoicesCache;

        // use an instance data context field because it should not be cached due to their internal caching
        private ProjectClocksContext db;

        public InvoiceRepository(ProjectClocksContext injectedContext)
        {
            db = injectedContext;

            // pre-load Invoices from the database as a normal Dictionary with Id as the key, then convert to a thread-safe ConcurrentDictionary
            if (invoicesCache is null)
            {
                invoicesCache = new ConcurrentDictionary<int, Invoice>(db.Invoices.ToDictionary(invoice => invoice.Id));
            }
        }

        public async Task<Invoice?> CreateAsync(Invoice invoice)
        {
            // add to database using EF Core
            EntityEntry<Invoice> added = await db.Invoices.AddAsync(invoice);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                if (invoicesCache is null) return invoice;
                // if the Invoice is new, add it to cache, else call UpdateCache method
                return invoicesCache.AddOrUpdate(invoice.Id, invoice, UpdateCache);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool?> DeleteAsync(int id)
        {
            // remove from database
            Invoice? invoice = db.Invoices.Find(id);
            if (invoice is null) return null;
            db.Invoices.Remove(invoice);
            int affected = await db.SaveChangesAsync();

            if (affected == 1)
            {
                if (invoicesCache is null) return null;
                // remove from cache
                return invoicesCache.TryRemove(id, out invoice);
            }
            else
            {
                return null;
            }
        }

        public Task<IEnumerable<Invoice>> RetrieveAllAsync()
        {
            // for performance, get from cache
            return Task.FromResult(invoicesCache is null ? Enumerable.Empty<Invoice>() : invoicesCache.Values);
        }

        public Task<Invoice?> RetrieveAsync(int id)
        {
            // for performance, get from cache
            if (invoicesCache is null) return null!;
            invoicesCache.TryGetValue(id, out Invoice? invoice);
            return Task.FromResult(invoice);
        }

        public async Task<Invoice?> UpdateAsync(int id, Invoice invoice)
        {
            // update in database
            db.Invoices.Update(invoice);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                // update in cache
                return UpdateCache(id, invoice);
            }
            return null;
        }

        private Invoice UpdateCache(int id, Invoice invoice)
        {
            Invoice? old;
            if (invoicesCache is not null)
            {
                if (invoicesCache.TryGetValue(id, out old))
                {
                    if (invoicesCache.TryUpdate(id, invoice, old))
                    {
                        return invoice;
                    }
                }
            }
            return null!;
        }
    }
}
