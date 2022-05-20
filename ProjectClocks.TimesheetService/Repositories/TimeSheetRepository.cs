using Microsoft.EntityFrameworkCore.ChangeTracking; // EntityEntry<T>
using ProjectClocks.Common.EntityModels.SqlServer; // TimeSheet
using System.Collections.Concurrent; // ConcurrentDictionary
using Task = System.Threading.Tasks.Task;

namespace ProjectClocks.TimesheetService.Repositories
{
    public class TimeSheetRepository : ITimeSheetRepository
    {
        // use a static thread-safe dictionary field to cache the timesheet entries
        // TODO: use Redis instead?
        private static ConcurrentDictionary<int, TimeSheet>? timeSheetCache;

        // use an instance data context field because it should not be cached due to their internal caching
        private ProjectClocksContext db;

        public TimeSheetRepository(ProjectClocksContext injectedContext)
        {
            db = injectedContext;

            // pre-load timesheet entries from the database as a normal Dictionary with Id as the key, then convert to a thread-safe ConcurrentDictionary
            if (timeSheetCache is null)
            {
                timeSheetCache = new ConcurrentDictionary<int, TimeSheet>(db.TimeSheets.ToDictionary(timeSheet => timeSheet.Id));
            }
        }

        public async Task<TimeSheet?> CreateAsync(TimeSheet timeSheet)
        {
            // add to database using EF Core
            EntityEntry<TimeSheet> added = await db.TimeSheets.AddAsync(timeSheet);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                if (timeSheetCache is null) return timeSheet;
                // if the timeSheet is new, add it to cache, else call UpdateCache method
                return timeSheetCache.AddOrUpdate(timeSheet.Id, timeSheet, UpdateCache);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool?> DeleteAsync(int id)
        {
            // remove from database
            TimeSheet? timeSheet = db.TimeSheets.Find(id);
            if (timeSheet is null) return null;
            db.TimeSheets.Remove(timeSheet);
            long affected = await db.SaveChangesAsync();

            if (affected == 1)
            {
                if (timeSheetCache is null) return null;
                // remove from cache
                return timeSheetCache.TryRemove(id, out timeSheet);
            }
            else
            {
                return null;
            }
        }

        public Task<IEnumerable<TimeSheet>> RetrieveAllAsync()
        {
            // for performance, get from cache
            return Task.FromResult(timeSheetCache is null ? Enumerable.Empty<TimeSheet>() : timeSheetCache.Values);
        }

        public Task<TimeSheet?> RetrieveAsync(int id)
        {
            // for performance, get from cache
            if (timeSheetCache is null) return null!;
            timeSheetCache.TryGetValue(id, out TimeSheet? timeSheet);
            return Task.FromResult(timeSheet);
        }

        public async Task<IEnumerable<TimeSheet>> RetrieveByGroupAsync(int id)
        {
            // for performance, get from cache
            if (timeSheetCache is null) return Enumerable.Empty<TimeSheet>();

            // query the dictionary by group_id and return all timesheets belonging to that group
            IEnumerable<TimeSheet> groupTimeSheets = new List<TimeSheet>();
            groupTimeSheets = db.TimeSheets.Where(g => g.GroupId == id).ToList();
            return await Task.FromResult(groupTimeSheets);
        }

        public async Task<IEnumerable<TimeSheet>> RetrieveByUserAsync(int id)
        {
            // for performance, get from cache
            if (timeSheetCache is null) return Enumerable.Empty<TimeSheet>();

            // query the dictionary by user_id and return all timesheets belonging to that user
            IEnumerable<TimeSheet> userTimeSheets = new List<TimeSheet>();
            userTimeSheets = db.TimeSheets.Where(g => g.UserId == id).ToList();
            return await Task.FromResult(userTimeSheets);
        }

        public async Task<IEnumerable<TimeSheet>> RetrieveByClientAsync(int id)
        {
            // for performance, get from cache
            if (timeSheetCache is null) return Enumerable.Empty<TimeSheet>();

            // query the dictionary by client_id and return all timesheets belonging to that client
            IEnumerable<TimeSheet> clientTimeSheets = new List<TimeSheet>();
            clientTimeSheets = db.TimeSheets.Where(g => g.ClientId == id).ToList();
            return await Task.FromResult(clientTimeSheets);
        }

        public async Task<IEnumerable<TimeSheet>> RetrieveByProjectAsync(int id)
        {
            // for performance, get from cache
            if (timeSheetCache is null) return Enumerable.Empty<TimeSheet>();

            // query the dictionary by project_id and return all timesheets belonging to that project
            IEnumerable<TimeSheet> projectTimeSheets = new List<TimeSheet>();
            projectTimeSheets = db.TimeSheets.Where(g => g.ProjectId == id).ToList();
            return await Task.FromResult(projectTimeSheets);
        }

        public async Task<TimeSheet?> UpdateAsync(int id, TimeSheet timeSheet)
        {
            // update in database
            db.TimeSheets.Update(timeSheet);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                // update in cache
                return UpdateCache(id, timeSheet);
            }
            return null;
        }

        private TimeSheet UpdateCache(int id, TimeSheet timeSheet)
        {
            TimeSheet? old;
            if (timeSheetCache is not null)
            {
                if (timeSheetCache.TryGetValue(id, out old))
                {
                    if (timeSheetCache.TryUpdate(id, timeSheet, old))
                    {
                        return timeSheet;
                    }
                }
            }
            return null!;
        }
    }
}
