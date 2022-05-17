using Microsoft.EntityFrameworkCore.ChangeTracking; // EntityEntry<T>
using ProjectClocks.Common.EntityModels.SqlServer; // ReportConfiguration
using System.Collections.Concurrent; // ConcurrentDictionary
using Task = System.Threading.Tasks.Task;

namespace ProjectClocks.ReportConfigurationService.Repositories
{
    public class ReportConfigurationRepository : IReportConfigurationRepository
    {
        // use a static thread-safe dictionary field to cache the report configurations
        // TODO: use Redis instead?
        private static ConcurrentDictionary<int, ReportConfiguration>? rcCache;

        // use an instance data context field because it should not be cached due to their internal caching
        private ProjectClocksContext db;

        public ReportConfigurationRepository(ProjectClocksContext injectedContext)
        {
            db = injectedContext;

            // pre-load ReportConfigurations from the database as a normal Dictionary with CustomerId as the key, then convert to a thread-safe ConcurrentDictionary
            if (rcCache is null)
            {
                rcCache = new ConcurrentDictionary<int, ReportConfiguration>(db.ReportConfigurations.ToDictionary(rc => rc.Id));
            }
        }

        public async Task<ReportConfiguration?> CreateAsync(ReportConfiguration rc)
        {
            // add to database using EF Core
            EntityEntry<ReportConfiguration> added = await db.ReportConfigurations.AddAsync(rc);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                if (rcCache is null) return rc;
                // if the ReportConfiguration is new, add it to cache, else call UpdateCache method
                return rcCache.AddOrUpdate(rc.Id, rc, UpdateCache);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool?> DeleteAsync(int id)
        {
            // remove from database
            ReportConfiguration? rc = db.ReportConfigurations.Find(id);
            if (rc is null) return null;
            db.ReportConfigurations.Remove(rc);
            int affected = await db.SaveChangesAsync();

            if (affected == 1)
            {
                if (rcCache is null) return null;
                // remove from cache
                return rcCache.TryRemove(id, out rc);
            }
            else
            {
                return null;
            }
        }

        public Task<IEnumerable<ReportConfiguration>> RetrieveAllAsync()
        {
            // for performance, get from cache
            return Task.FromResult(rcCache is null ? Enumerable.Empty<ReportConfiguration>() : rcCache.Values);
        }

        public Task<ReportConfiguration?> RetrieveAsync(int id)
        {
            // for performance, get from cache
            if (rcCache is null) return null!;
            rcCache.TryGetValue(id, out ReportConfiguration? rc);
            return Task.FromResult(rc);
        }

        public async Task<ReportConfiguration?> UpdateAsync(int id, ReportConfiguration rc)
        {
            // update in database
            db.ReportConfigurations.Update(rc);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                // update in cache
                return UpdateCache(id, rc);
            }
            return null;
        }

        private ReportConfiguration UpdateCache(int id, ReportConfiguration rc)
        {
            ReportConfiguration? old;
            if (rcCache is not null)
            {
                if (rcCache.TryGetValue(id, out old))
                {
                    if (rcCache.TryUpdate(id, rc, old))
                    {
                        return rc;
                    }
                }
            }
            return null!;
        }
    }
}
