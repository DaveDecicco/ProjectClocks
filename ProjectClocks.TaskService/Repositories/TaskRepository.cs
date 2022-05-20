using Microsoft.EntityFrameworkCore.ChangeTracking; // EntityEntry<T>
using ProjectClocks.Common.EntityModels.SqlServer; // ProjectClocksContext
using System.Collections.Concurrent; // ConcurrentDictionary
using Task = System.Threading.Tasks.Task;
using PCTask = ProjectClocks.Common.EntityModels.SqlServer.Task; // Task

namespace ProjectClocks.TaskService.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        // use a static thread-safe dictionary field to cache the tasks
        // TODO: use Redis instead?
        private static ConcurrentDictionary<int, PCTask>? taskCache;

        // use an instance data context field because it should not be cached due to their internal caching
        private ProjectClocksContext db;

        public TaskRepository(ProjectClocksContext injectedContext)
        {
            db = injectedContext;

            // pre-load tasks from the database as a normal Dictionary with Id as the key, then convert to a thread-safe ConcurrentDictionary
            if (taskCache is null)
            {
                taskCache = new ConcurrentDictionary<int, PCTask>(db.Tasks.ToDictionary(client => client.Id));
            }
        }

        public async Task<PCTask?> CreateAsync(PCTask client)
        {
            // add to database using EF Core
            EntityEntry<PCTask> added = await db.Tasks.AddAsync(client);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                if (taskCache is null) return client;
                // if the client is new, add it to cache, else call UpdateCache method
                return taskCache.AddOrUpdate(client.Id, client, UpdateCache);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool?> DeleteAsync(int id)
        {
            // remove from database
            PCTask? client = db.Tasks.Find(id);
            if (client is null) return null;
            db.Tasks.Remove(client);
            int affected = await db.SaveChangesAsync();

            if (affected == 1)
            {
                if (taskCache is null) return null;
                // remove from cache
                return taskCache.TryRemove(id, out client);
            }
            else
            {
                return null;
            }
        }

        public Task<IEnumerable<PCTask>> RetrieveAllAsync()
        {
            // for performance, get from cache
            return Task.FromResult(taskCache is null ? Enumerable.Empty<PCTask>() : taskCache.Values);
        }

        public Task<PCTask?> RetrieveAsync(int id)
        {
            // for performance, get from cache
            if (taskCache is null) return null!;
            taskCache.TryGetValue(id, out PCTask? client);
            return Task.FromResult(client);
        }

        public async Task<PCTask?> UpdateAsync(int id, PCTask client)
        {
            // update in database
            db.Tasks.Update(client);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                // update in cache
                return UpdateCache(id, client);
            }
            return null;
        }

        private PCTask UpdateCache(int id, PCTask client)
        {
            PCTask? old;
            if (taskCache is not null)
            {
                if (taskCache.TryGetValue(id, out old))
                {
                    if (taskCache.TryUpdate(id, client, old))
                    {
                        return client;
                    }
                }
            }
            return null!;
        }
    }
}
