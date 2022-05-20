using ProjectClocks.Common.EntityModels.SqlServer;

namespace ProjectClocks.UserTimeEntryService.Repositories
{
    public interface IUserTimeEntryRepository
    {
        Task<UserTimeEntry?> CreateAsync(UserTimeEntry userTimeEntry);
        Task<IEnumerable<UserTimeEntry>> RetrieveAllAsync();
        Task<UserTimeEntry?> RetrieveAsync(long id);
        Task<IEnumerable<UserTimeEntry>> RetrieveByGroupAsync(int id);
        Task<IEnumerable<UserTimeEntry>> RetrieveByUserAsync(int id);
        Task<IEnumerable<UserTimeEntry>> RetrieveByClientAsync(int id);
        Task<IEnumerable<UserTimeEntry>> RetrieveByProjectAsync(int id);
        Task<IEnumerable<UserTimeEntry>> RetrieveByTaskAsync(int id);
        Task<UserTimeEntry?> UpdateAsync(long id, UserTimeEntry userTimeEntry);
        Task<bool?> DeleteAsync(long id);
    }
}
