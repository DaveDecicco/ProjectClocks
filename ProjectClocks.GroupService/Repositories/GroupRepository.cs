using Microsoft.EntityFrameworkCore.ChangeTracking; // EntityEntry<T>
using ProjectClocks.Common.EntityModels.SqlServer; // Group
using System.Collections.Concurrent; // ConcurrentDictionary
using Task = System.Threading.Tasks.Task;

namespace ProjectClocks.GroupService.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        // use a static thread-safe dictionary field to cache the Groups
        // TODO: use Redis instead?
        private static ConcurrentDictionary<int, Group>? groupsCache;

        // use an instance data context field because it should not be cached due to their internal caching
        private ProjectClocksContext db;

        public GroupRepository(ProjectClocksContext injectedContext)
        {
            db = injectedContext;

            // pre-load Groups from the database as a normal Dictionary with CustomerId as the key, then convert to a thread-safe ConcurrentDictionary
            if (groupsCache is null)
            {
                groupsCache = new ConcurrentDictionary<int, Group>(db.Groups.ToDictionary(group => group.Id));
            }
        }

        public async Task<Group?> CreateAsync(Group group)
        {
            // add to database using EF Core
            EntityEntry<Group> added = await db.Groups.AddAsync(group);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                if (groupsCache is null) return group;
                // if the Group is new, add it to cache, else call UpdateCache method
                return groupsCache.AddOrUpdate(group.Id, group, UpdateCache);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool?> DeleteAsync(int id)
        {
            // remove from database
            Group? group = db.Groups.Find(id);
            if (group is null) return null;
            db.Groups.Remove(group);
            int affected = await db.SaveChangesAsync();

            if (affected == 1)
            {
                if (groupsCache is null) return null;
                // remove from cache
                return groupsCache.TryRemove(id, out group);
            }
            else
            {
                return null;
            }
        }

        public Task<IEnumerable<Group>> RetrieveAllAsync()
        {
            // for performance, get from cache
            return Task.FromResult(groupsCache is null ? Enumerable.Empty<Group>() : groupsCache.Values);
        }

        public Task<Group?> RetrieveAsync(int id)
        {
            // for performance, get from cache
            if (groupsCache is null) return null!;
            groupsCache.TryGetValue(id, out Group? group);
            return Task.FromResult(group);
        }

        public async Task<Group?> UpdateAsync(int id, Group group)
        {
            // update in database
            db.Groups.Update(group);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                // update in cache
                return UpdateCache(id, group);
            }
            return null;
        }

        private Group UpdateCache(int id, Group group)
        {
            Group? old;
            if (groupsCache is not null)
            {
                if (groupsCache.TryGetValue(id, out old))
                {
                    if (groupsCache.TryUpdate(id, group, old))
                    {
                        return group;
                    }
                }
            }
            return null!;
        }
    }
}
