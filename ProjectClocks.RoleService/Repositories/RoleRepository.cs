using Microsoft.EntityFrameworkCore.ChangeTracking; // EntityEntry<T>
using ProjectClocks.Common.EntityModels.SqlServer; // Role
using System.Collections.Concurrent; // ConcurrentDictionary
using Task = System.Threading.Tasks.Task;

namespace ProjectClocks.RoleService.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        // use a static thread-safe dictionary field to cache the roles
        // TODO: use Redis instead?
        private static ConcurrentDictionary<int, Role>? roleCache;

        // use an instance data context field because it should not be cached due to their internal caching
        private ProjectClocksContext db;

        public RoleRepository(ProjectClocksContext injectedContext)
        {
            db = injectedContext;

            // pre-load roles from the database as a normal Dictionary with Id as the key, then convert to a thread-safe ConcurrentDictionary
            if (roleCache is null)
            {
                roleCache = new ConcurrentDictionary<int, Role>(db.Roles.ToDictionary(role => role.Id));
            }
        }

        public async Task<Role?> CreateAsync(Role role)
        {
            // add to database using EF Core
            EntityEntry<Role> added = await db.Roles.AddAsync(role);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                if (roleCache is null) return role;
                // if the role is new, add it to cache, else call UpdateCache method
                return roleCache.AddOrUpdate(role.Id, role, UpdateCache);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool?> DeleteAsync(int id)
        {
            // remove from database
            Role? role = db.Roles.Find(id);
            if (role is null) return null;
            db.Roles.Remove(role);
            int affected = await db.SaveChangesAsync();

            if (affected == 1)
            {
                if (roleCache is null) return null;
                // remove from cache
                return roleCache.TryRemove(id, out role);
            }
            else
            {
                return null;
            }
        }

        public Task<IEnumerable<Role>> RetrieveAllAsync()
        {
            // for performance, get from cache
            return Task.FromResult(roleCache is null ? Enumerable.Empty<Role>() : roleCache.Values);
        }

        public Task<Role?> RetrieveAsync(int id)
        {
            // for performance, get from cache
            if (roleCache is null) return null!;
            roleCache.TryGetValue(id, out Role? role);
            return Task.FromResult(role);
        }

        public async Task<Role?> UpdateAsync(int id, Role role)
        {
            // update in database
            db.Roles.Update(role);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                // update in cache
                return UpdateCache(id, role);
            }
            return null;
        }

        private Role UpdateCache(int id, Role role)
        {
            Role? old;
            if (roleCache is not null)
            {
                if (roleCache.TryGetValue(id, out old))
                {
                    if (roleCache.TryUpdate(id, role, old))
                    {
                        return role;
                    }
                }
            }
            return null!;
        }
    }
}
