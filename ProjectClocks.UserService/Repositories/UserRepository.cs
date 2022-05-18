using Microsoft.EntityFrameworkCore.ChangeTracking; // EntityEntry<T>
using ProjectClocks.Common.EntityModels.SqlServer; // User
using System.Collections.Concurrent; // ConcurrentDictionary
using Task = System.Threading.Tasks.Task;

namespace ProjectClocks.UserService.Repositories
{
    public class UserRepository : IUserRepository
    {
        // use a static thread-safe dictionary field to cache the users
        // TODO: use Redis instead?
        private static ConcurrentDictionary<int, User>? usersCache;

        // use an instance data context field because it should not be cached due to their internal caching
        private ProjectClocksContext db;

        public UserRepository(ProjectClocksContext injectedContext)
        {
            db = injectedContext;

            // pre-load users from the database as a normal Dictionary with CustomerId as the key, then convert to a thread-safe ConcurrentDictionary
            if (usersCache is null)
            {
                usersCache = new ConcurrentDictionary<int, User>(db.Users.ToDictionary(user => user.Id));
            }
        }

        public async Task<User?> CreateAsync(User user)
        {
            // add to database using EF Core
            EntityEntry<User> added = await db.Users.AddAsync(user);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                if (usersCache is null) return user;
                // if the user is new, add it to cache, else call UpdateCache method
                return usersCache.AddOrUpdate(user.Id, user, UpdateCache);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool?> DeleteAsync(int id)
        {
            // remove from database
            User? user = db.Users.Find(id);
            if (user is null) return null;
            db.Users.Remove(user);
            int affected = await db.SaveChangesAsync();

            if (affected == 1)
            {
                if (usersCache is null) return null;
                // remove from cache
                return usersCache.TryRemove(id, out user);
            }
            else
            {
                return null;
            }
        }

        public Task<IEnumerable<User>> RetrieveAllAsync()
        {
            // for performance, get from cache
            return Task.FromResult(usersCache is null ? Enumerable.Empty<User>() : usersCache.Values);
        }

        public Task<User?> RetrieveAsync(int id)
        {
            // for performance, get from cache
            if (usersCache is null) return null!;
            usersCache.TryGetValue(id, out User? user);
            return Task.FromResult(user);
        }

        public async Task<IEnumerable<User>> RetrieveByGroupAsync(int id)
        {
            // for performance, get from cache
            if (usersCache is null) return Enumerable.Empty<User>();

            // query the dictionary by group_id and return all users belonging to that group
            IEnumerable<User> groupUsers = new List<User>();
            groupUsers = db.Users.Where(g => g.GroupId == id).ToList();
            return await Task.FromResult(groupUsers);
        }

        public async Task<IEnumerable<User>> RetrieveByRoleAsync(int id)
        {
            // for performance, get from cache
            if (usersCache is null) return Enumerable.Empty<User>();

            // query the dictionary by role_id and return all users belonging to that role
            IEnumerable<User> roleUsers = new List<User>();
            roleUsers = db.Users.Where(g => g.RoleId == id).ToList();
            return await Task.FromResult(roleUsers);
        }

        public async Task<IEnumerable<User>> RetrieveByClientAsync(int id)
        {
            // for performance, get from cache
            if (usersCache is null) return Enumerable.Empty<User>();

            // query the dictionary by client_id and return all users belonging to that client
            IEnumerable<User> clientUsers = new List<User>();
            clientUsers = db.Users.Where(g => g.ClientId == id).ToList();
            return await Task.FromResult(clientUsers);
        }

        public async Task<User?> UpdateAsync(int id, User user)
        {
            // update in database
            db.Users.Update(user);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                // update in cache
                return UpdateCache(id, user);
            }
            return null;
        }

        private User UpdateCache(int id, User user)
        {
            User? old;
            if (usersCache is not null)
            {
                if (usersCache.TryGetValue(id, out old))
                {
                    if (usersCache.TryUpdate(id, user, old))
                    {
                        return user;
                    }
                }
            }
            return null!;
        }
    }
}
