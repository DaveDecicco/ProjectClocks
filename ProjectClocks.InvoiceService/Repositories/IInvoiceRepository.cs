using ProjectClocks.Common.EntityModels.SqlServer;

namespace ProjectClocks.InvoiceService.Repositories
{
    public interface IInvoiceRepository
    {
        Task<Invoice?> CreateAsync(Invoice invoice);
        Task<IEnumerable<Invoice>> RetrieveAllAsync();
        Task<Invoice?> RetrieveAsync(int id);
        Task<Invoice?> UpdateAsync(int id, Invoice invoice);
        Task<bool?> DeleteAsync(int id);
    }
}
