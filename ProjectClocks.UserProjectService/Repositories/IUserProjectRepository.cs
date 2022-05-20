using ProjectClocks.Common.EntityModels.SqlServer;

namespace ProjectClocks.UserProjectService.Repositories
{
    public interface IUserProjectRepository
    {
        Task<UserProject?> CreateAsync(UserProject user);
        Task<IEnumerable<UserProject>> RetrieveAllAsync();
        Task<UserProject?> RetrieveAsync(int id);
        Task<IEnumerable<UserProject>> RetrieveByGroupAsync(int id);
        Task<IEnumerable<UserProject>> RetrieveByUserAsync(int id);
        Task<UserProject?> UpdateAsync(int id, UserProject userProject);
        Task<bool?> DeleteAsync(int id);
    }
}
