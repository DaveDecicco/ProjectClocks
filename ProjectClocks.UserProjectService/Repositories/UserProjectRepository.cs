using Microsoft.EntityFrameworkCore.ChangeTracking; // EntityEntry<T>
using ProjectClocks.Common.EntityModels.SqlServer; // UserProject
using System.Collections.Concurrent; // ConcurrentDictionary
using Task = System.Threading.Tasks.Task;

namespace ProjectClocks.UserProjectService.Repositories
{
    public class UserProjectRepository : IUserProjectRepository
    {
        // use a static thread-safe dictionary field to cache the user projects
        // TODO: use Redis instead?
        private static ConcurrentDictionary<int, UserProject>? usersProjectsCache;

        // use an instance data context field because it should not be cached due to their internal caching
        private ProjectClocksContext db;

        public UserProjectRepository(ProjectClocksContext injectedContext)
        {
            db = injectedContext;

            // pre-load user projects from the database as a normal Dictionary with Id as the key, then convert to a thread-safe ConcurrentDictionary
            if (usersProjectsCache is null)
            {
                usersProjectsCache = new ConcurrentDictionary<int, UserProject>(db.UserProjects.ToDictionary(userproject => userproject.Id));
            }
        }

        public async Task<UserProject?> CreateAsync(UserProject userproject)
        {
            // add to database using EF Core
            EntityEntry<UserProject> added = await db.UserProjects.AddAsync(userproject);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                if (usersProjectsCache is null) return userproject;
                // if the userproject is new, add it to cache, else call UpdateCache method
                return usersProjectsCache.AddOrUpdate(userproject.Id, userproject, UpdateCache);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool?> DeleteAsync(int id)
        {
            // remove from database
            UserProject? userproject = db.UserProjects.Find(id);
            if (userproject is null) return null;
            db.UserProjects.Remove(userproject);
            int affected = await db.SaveChangesAsync();

            if (affected == 1)
            {
                if (usersProjectsCache is null) return null;
                // remove from cache
                return usersProjectsCache.TryRemove(id, out userproject);
            }
            else
            {
                return null;
            }
        }

        public Task<IEnumerable<UserProject>> RetrieveAllAsync()
        {
            // for performance, get from cache
            return Task.FromResult(usersProjectsCache is null ? Enumerable.Empty<UserProject>() : usersProjectsCache.Values);
        }

        public Task<UserProject?> RetrieveAsync(int id)
        {
            // for performance, get from cache
            if (usersProjectsCache is null) return null!;
            usersProjectsCache.TryGetValue(id, out UserProject? userproject);
            return Task.FromResult(userproject);
        }

        public async Task<IEnumerable<UserProject>> RetrieveByGroupAsync(int id)
        {
            // for performance, get from cache
            if (usersProjectsCache is null) return Enumerable.Empty<UserProject>();

            // query the dictionary by group_id and return all userprojects belonging to that group
            IEnumerable<UserProject> groupUserProjects = new List<UserProject>();
            groupUserProjects = db.UserProjects.Where(g => g.GroupId == id).ToList();
            return await Task.FromResult(groupUserProjects);
        }

        public async Task<IEnumerable<UserProject>> RetrieveByUserAsync(int id)
        {
            // for performance, get from cache
            if (usersProjectsCache is null) return Enumerable.Empty<UserProject>();

            // query the dictionary by user_id and return all users belonging to that role
            IEnumerable<UserProject> userProjects = new List<UserProject>();
            userProjects = db.UserProjects.Where(g => g.UserId == id).ToList();
            return await Task.FromResult(userProjects);
        }

        public async Task<UserProject?> UpdateAsync(int id, UserProject userproject)
        {
            // update in database
            db.UserProjects.Update(userproject);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                // update in cache
                return UpdateCache(id, userproject);
            }
            return null;
        }

        private UserProject UpdateCache(int id, UserProject userproject)
        {
            UserProject? old;
            if (usersProjectsCache is not null)
            {
                if (usersProjectsCache.TryGetValue(id, out old))
                {
                    if (usersProjectsCache.TryUpdate(id, userproject, old))
                    {
                        return userproject;
                    }
                }
            }
            return null!;
        }
    }
}
