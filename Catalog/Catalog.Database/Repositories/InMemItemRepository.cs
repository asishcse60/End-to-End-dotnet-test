using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Contracts.Models;

namespace Catalog.Database.Repositories
{
    public class InMemItemRepository : IItemRepository
    {
        private readonly List<Item> items = new List<Item>()
        {
            new Item {Name = "Potion", Price = 9, CreatedDate = DateTimeOffset.UtcNow},
            new Item {Name = "Iron Sword", Price = 20, CreatedDate = DateTimeOffset.UtcNow},
            new Item {Name = "Bronze Shield", Price = 18, CreatedDate = DateTimeOffset.UtcNow},
        };
       
        public async Task<Item> GetItemAsync(string id)
        {
            var item = items.SingleOrDefault(item1 => item1.Id == id);
            return await Task.FromResult(item);
        }

        public async Task<IList<Item>> GetItemsAsync()
        {
            return await Task.FromResult(items);
        }

        public async Task CreateItemAsync(Item item)
        {
            items.Add(item);
            await Task.CompletedTask;
        }

        public async Task UpdateItemAsync(Item item)
        {
            var index = items.FindIndex(existingItem => existingItem.Id == item.Id);
            items[index] = item;
            await Task.CompletedTask;
        }

        public async Task DeleteItemAsync(string id)
        {
            var index = items.FindIndex(existingItem => existingItem.Id == id);
            items.RemoveAt(index);
            await Task.CompletedTask;
        }
    }
}
