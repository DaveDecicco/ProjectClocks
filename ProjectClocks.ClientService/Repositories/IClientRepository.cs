using ProjectClocks.Common.EntityModels.SqlServer;

namespace ProjectClocks.ClientService.Repositories
{
    public interface IClientRepository
    {
        Task<Client?> CreateAsync(Client client);
        Task<IEnumerable<Client>> RetrieveAllAsync();
        Task<Client?> RetrieveAsync(int id);
        Task<Client?> UpdateAsync(int id, Client client);
        Task<bool?> DeleteAsync(int id);
    }
}
