using ProjectClocks.Common.EntityModels.SqlServer;

namespace ProjectClocks.ReportConfigurationService.Repositories
{
    public interface IReportConfigurationRepository
    {
        Task<ReportConfiguration?> CreateAsync(ReportConfiguration rc);
        Task<IEnumerable<ReportConfiguration>> RetrieveAllAsync();
        Task<ReportConfiguration?> RetrieveAsync(int id);
        Task<ReportConfiguration?> UpdateAsync(int id, ReportConfiguration rc);
        Task<bool?> DeleteAsync(int id);
    }
}
