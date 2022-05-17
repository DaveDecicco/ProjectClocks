using PCTask = ProjectClocks.Common.EntityModels.SqlServer.Task;

namespace ProjectClocks.TaskService.Repositories
{
    public interface ITaskRepository
    {
        Task<PCTask?> CreateAsync(PCTask task);
        Task<IEnumerable<PCTask>> RetrieveAllAsync();
        Task<PCTask?> RetrieveAsync(int id);
        Task<PCTask?> UpdateAsync(int id, PCTask task);
        Task<bool?> DeleteAsync(int id);
    }
}
