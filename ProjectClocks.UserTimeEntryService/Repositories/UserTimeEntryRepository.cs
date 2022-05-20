using Microsoft.EntityFrameworkCore.ChangeTracking; // EntityEntry<T>
using ProjectClocks.Common.EntityModels.SqlServer; // UserTimeEntry
using System.Collections.Concurrent; // ConcurrentDictionary
using Task = System.Threading.Tasks.Task;

namespace ProjectClocks.UserTimeEntryService.Repositories
{
    public class UserTimeEntryRepository : IUserTimeEntryRepository
    {
        // use a static thread-safe dictionary field to cache the user time entries
        // TODO: use Redis instead?
        private static ConcurrentDictionary<long, UserTimeEntry>? userTimeEntriesCache;

        // use an instance data context field because it should not be cached due to their internal caching
        private ProjectClocksContext db;

        public UserTimeEntryRepository(ProjectClocksContext injectedContext)
        {
            db = injectedContext;

            // pre-load user time entries from the database as a normal Dictionary with Id as the key, then convert to a thread-safe ConcurrentDictionary
            if (userTimeEntriesCache is null)
            {
                userTimeEntriesCache = new ConcurrentDictionary<long, UserTimeEntry>(db.UserTimeEntries.ToDictionary(userTimeEntry => userTimeEntry.Id));
            }
        }

        public async Task<UserTimeEntry?> CreateAsync(UserTimeEntry userTimeEntry)
        {
            // add to database using EF Core
            EntityEntry<UserTimeEntry> added = await db.UserTimeEntries.AddAsync(userTimeEntry);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                if (userTimeEntriesCache is null) return userTimeEntry;
                // if the userTimeEntry is new, add it to cache, else call UpdateCache method
                return userTimeEntriesCache.AddOrUpdate(userTimeEntry.Id, userTimeEntry, UpdateCache);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool?> DeleteAsync(long id)
        {
            // remove from database
            UserTimeEntry? userTimeEntry = db.UserTimeEntries.Find(id);
            if (userTimeEntry is null) return null;
            db.UserTimeEntries.Remove(userTimeEntry);
            long affected = await db.SaveChangesAsync();

            if (affected == 1)
            {
                if (userTimeEntriesCache is null) return null;
                // remove from cache
                return userTimeEntriesCache.TryRemove(id, out userTimeEntry);
            }
            else
            {
                return null;
            }
        }

        public Task<IEnumerable<UserTimeEntry>> RetrieveAllAsync()
        {
            // for performance, get from cache
            return Task.FromResult(userTimeEntriesCache is null ? Enumerable.Empty<UserTimeEntry>() : userTimeEntriesCache.Values);
        }

        public Task<UserTimeEntry?> RetrieveAsync(long id)
        {
            // for performance, get from cache
            if (userTimeEntriesCache is null) return null!;
            userTimeEntriesCache.TryGetValue(id, out UserTimeEntry? userTimeEntry);
            return Task.FromResult(userTimeEntry);
        }

        public async Task<IEnumerable<UserTimeEntry>> RetrieveByGroupAsync(int id)
        {
            // for performance, get from cache
            if (userTimeEntriesCache is null) return Enumerable.Empty<UserTimeEntry>();

            // query the dictionary by group_id and return all user time entries belonging to that group
            IEnumerable<UserTimeEntry> groupUserTimeEntries = new List<UserTimeEntry>();
            groupUserTimeEntries = db.UserTimeEntries.Where(g => g.GroupId == id).ToList();
            return await Task.FromResult(groupUserTimeEntries);
        }

        public async Task<IEnumerable<UserTimeEntry>> RetrieveByUserAsync(int id)
        {
            // for performance, get from cache
            if (userTimeEntriesCache is null) return Enumerable.Empty<UserTimeEntry>();

            // query the dictionary by user_id and return all user time entries belonging to that user
            IEnumerable<UserTimeEntry> roleUserTimeEntries = new List<UserTimeEntry>();
            roleUserTimeEntries = db.UserTimeEntries.Where(g => g.UserId == id).ToList();
            return await Task.FromResult(roleUserTimeEntries);
        }

        public async Task<IEnumerable<UserTimeEntry>> RetrieveByClientAsync(int id)
        {
            // for performance, get from cache
            if (userTimeEntriesCache is null) return Enumerable.Empty<UserTimeEntry>();

            // query the dictionary by client_id and return all user time entries belonging to that client
            IEnumerable<UserTimeEntry> clientUserTimeEntries = new List<UserTimeEntry>();
            clientUserTimeEntries = db.UserTimeEntries.Where(g => g.ClientId == id).ToList();
            return await Task.FromResult(clientUserTimeEntries);
        }

        public async Task<IEnumerable<UserTimeEntry>> RetrieveByProjectAsync(int id)
        {
            // for performance, get from cache
            if (userTimeEntriesCache is null) return Enumerable.Empty<UserTimeEntry>();

            // query the dictionary by project_id and return all user time entries belonging to that project
            IEnumerable<UserTimeEntry> projectUserTimeEntries = new List<UserTimeEntry>();
            projectUserTimeEntries = db.UserTimeEntries.Where(g => g.ProjectId == id).ToList();
            return await Task.FromResult(projectUserTimeEntries);
        }

        public async Task<IEnumerable<UserTimeEntry>> RetrieveByTaskAsync(int id)
        {
            // for performance, get from cache
            if (userTimeEntriesCache is null) return Enumerable.Empty<UserTimeEntry>();

            // query the dictionary by task_id and return all user time entries belonging to that task
            IEnumerable<UserTimeEntry> taskUserTimeEntries = new List<UserTimeEntry>();
            taskUserTimeEntries = db.UserTimeEntries.Where(g => g.TaskId == id).ToList();
            return await Task.FromResult(taskUserTimeEntries);
        }

        public async Task<UserTimeEntry?> UpdateAsync(long id, UserTimeEntry userTimeEntry)
        {
            // update in database
            db.UserTimeEntries.Update(userTimeEntry);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                // update in cache
                return UpdateCache(id, userTimeEntry);
            }
            return null;
        }

        private UserTimeEntry UpdateCache(long id, UserTimeEntry userTimeEntry)
        {
            UserTimeEntry? old;
            if (userTimeEntriesCache is not null)
            {
                if (userTimeEntriesCache.TryGetValue(id, out old))
                {
                    if (userTimeEntriesCache.TryUpdate(id, userTimeEntry, old))
                    {
                        return userTimeEntry;
                    }
                }
            }
            return null!;
        }
    }
}
