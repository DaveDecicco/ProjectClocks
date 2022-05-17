using ProjectClocks.Common.EntityModels.SqlServer;

namespace ProjectClocks.ProjectService.Repositories
{
    public interface IProjectRepository
    {
        Task<Project?> CreateAsync(Project project);
        Task<IEnumerable<Project>> RetrieveAllAsync();
        Task<Project?> RetrieveAsync(int id);
        Task<Project?> UpdateAsync(int id, Project project);
        Task<bool?> DeleteAsync(int id);
    }
}
