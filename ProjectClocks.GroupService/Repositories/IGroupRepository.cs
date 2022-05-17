using ProjectClocks.Common.EntityModels.SqlServer;

namespace ProjectClocks.GroupService.Repositories
{
    public interface IGroupRepository
    {
        Task<Group?> CreateAsync(Group group);
        Task<IEnumerable<Group>> RetrieveAllAsync();
        Task<Group?> RetrieveAsync(int id);
        Task<Group?> UpdateAsync(int id, Group group);
        Task<bool?> DeleteAsync(int id);
    }
}
