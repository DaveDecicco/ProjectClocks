using ProjectClocks.Common.EntityModels.SqlServer;

namespace ProjectClocks.UserService.Repositories
{
    public interface IUserRepository
    {
        Task<User?> CreateAsync(User user);
        Task<IEnumerable<User>> RetrieveAllAsync();
        Task<User?> RetrieveAsync(int id);
        Task<IEnumerable<User>> RetrieveByGroupAsync(int id);
        Task<IEnumerable<User>> RetrieveByRoleAsync(int id);
        Task<IEnumerable<User>> RetrieveByClientAsync(int id);
        Task<User?> UpdateAsync(int id, User user);
        Task<bool?> DeleteAsync(int id);
    }
}
