using ProjectClocks.Common.EntityModels.SqlServer;

namespace ProjectClocks.TimesheetService.Repositories
{
    public interface ITimeSheetRepository
    {
        Task<TimeSheet?> CreateAsync(TimeSheet timeSheet);
        Task<IEnumerable<TimeSheet>> RetrieveAllAsync();
        Task<TimeSheet?> RetrieveAsync(int id);
        Task<IEnumerable<TimeSheet>> RetrieveByGroupAsync(int id);
        Task<IEnumerable<TimeSheet>> RetrieveByUserAsync(int id);
        Task<IEnumerable<TimeSheet>> RetrieveByClientAsync(int id);
        Task<IEnumerable<TimeSheet>> RetrieveByProjectAsync(int id);
        Task<TimeSheet?> UpdateAsync(int id, TimeSheet timeSheet);
        Task<bool?> DeleteAsync(int id);
    }
}
