using Microsoft.EntityFrameworkCore.ChangeTracking; // EntityEntry<T>
using ProjectClocks.Common.EntityModels.SqlServer; // Project
using System.Collections.Concurrent; // ConcurrentDictionary
using Task = System.Threading.Tasks.Task;

namespace ProjectClocks.ProjectService.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        // use a static thread-safe dictionary field to cache the Projects
        // TODO: use Redis instead?
        private static ConcurrentDictionary<int, Project>? projectsCache;

        // use an instance data context field because it should not be cached due to their internal caching
        private ProjectClocksContext db;

        public ProjectRepository(ProjectClocksContext injectedContext)
        {
            db = injectedContext;

            // pre-load Projects from the database as a normal Dictionary with Id as the key, then convert to a thread-safe ConcurrentDictionary
            if (projectsCache is null)
            {
                projectsCache = new ConcurrentDictionary<int, Project>(db.Projects.ToDictionary(project => project.Id));
            }
        }

        public async Task<Project?> CreateAsync(Project project)
        {
            // add to database using EF Core
            EntityEntry<Project> added = await db.Projects.AddAsync(project);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                if (projectsCache is null) return project;
                // if the Project is new, add it to cache, else call UpdateCache method
                return projectsCache.AddOrUpdate(project.Id, project, UpdateCache);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool?> DeleteAsync(int id)
        {
            // remove from database
            Project? project = db.Projects.Find(id);
            if (project is null) return null;
            db.Projects.Remove(project);
            int affected = await db.SaveChangesAsync();

            if (affected == 1)
            {
                if (projectsCache is null) return null;
                // remove from cache
                return projectsCache.TryRemove(id, out project);
            }
            else
            {
                return null;
            }
        }

        public Task<IEnumerable<Project>> RetrieveAllAsync()
        {
            // for performance, get from cache
            return Task.FromResult(projectsCache is null ? Enumerable.Empty<Project>() : projectsCache.Values);
        }

        public Task<Project?> RetrieveAsync(int id)
        {
            // for performance, get from cache
            if (projectsCache is null) return null!;
            projectsCache.TryGetValue(id, out Project? project);
            return Task.FromResult(project);
        }

        public async Task<Project?> UpdateAsync(int id, Project project)
        {
            // update in database
            db.Projects.Update(project);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                // update in cache
                return UpdateCache(id, project);
            }
            return null;
        }

        private Project UpdateCache(int id, Project project)
        {
            Project? old;
            if (projectsCache is not null)
            {
                if (projectsCache.TryGetValue(id, out old))
                {
                    if (projectsCache.TryUpdate(id, project, old))
                    {
                        return project;
                    }
                }
            }
            return null!;
        }
    }
}
