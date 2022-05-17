using Microsoft.EntityFrameworkCore.ChangeTracking; // EntityEntry<T>
using ProjectClocks.Common.EntityModels.SqlServer; // Client
using System.Collections.Concurrent; // ConcurrentDictionary
using Task = System.Threading.Tasks.Task;

namespace ProjectClocks.ClientService.Repositories
{
    public class ClientRepository : IClientRepository
    {
        // use a static thread-safe dictionary field to cache the clients
        // TODO: use Redis instead?
        private static ConcurrentDictionary<int, Client>? clientsCache;

        // use an instance data context field because it should not be cached due to their internal caching
        private ProjectClocksContext db;

        public ClientRepository(ProjectClocksContext injectedContext)
        {
            db = injectedContext;

            // pre-load clients from the database as a normal Dictionary with CustomerId as the key, then convert to a thread-safe ConcurrentDictionary
            if (clientsCache is null)
            {
                clientsCache = new ConcurrentDictionary<int, Client>(db.Clients.ToDictionary(client => client.Id));
            }
        }

        public async Task<Client?> CreateAsync(Client client)
        {
            // add to database using EF Core
            EntityEntry<Client> added = await db.Clients.AddAsync(client);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                if (clientsCache is null) return client;
                // if the client is new, add it to cache, else call UpdateCache method
                return clientsCache.AddOrUpdate(client.Id, client, UpdateCache);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool?> DeleteAsync(int id)
        {
            // remove from database
            Client? client = db.Clients.Find(id);
            if (client is null) return null;
            db.Clients.Remove(client);
            int affected = await db.SaveChangesAsync();

            if (affected == 1)
            {
                if (clientsCache is null) return null;
                // remove from cache
                return clientsCache.TryRemove(id, out client);
            }
            else
            {
                return null;
            }
        }

        public Task<IEnumerable<Client>> RetrieveAllAsync()
        {
            // for performance, get from cache
            return Task.FromResult(clientsCache is null ? Enumerable.Empty<Client>() : clientsCache.Values);
        }

        public Task<Client?> RetrieveAsync(int id)
        {
            // for performance, get from cache
            if (clientsCache is null) return null!;
            clientsCache.TryGetValue(id, out Client? client);
            return Task.FromResult(client);
        }

        public async Task<Client?> UpdateAsync(int id, Client client)
        {
            // update in database
            db.Clients.Update(client);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                // update in cache
                return UpdateCache(id, client);
            }
            return null;
        }

        private Client UpdateCache(int id, Client client)
        {
            Client? old;
            if (clientsCache is not null)
            {
                if (clientsCache.TryGetValue(id, out old))
                {
                    if (clientsCache.TryUpdate(id, client, old))
                    {
                        return client;
                    }
                }
            }
            return null!;
        }
    }
}
