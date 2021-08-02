using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Contracts.Models;

namespace Catalog.Database.Repositories
{
    public interface IItemRepository
    {
        Task<Item> GetItemAsync(string id);
        Task<IList<Item>> GetItemsAsync();
        Task CreateItemAsync(Item item);
        Task UpdateItemAsync(Item item);
        Task DeleteItemAsync(string id);
    }
}
