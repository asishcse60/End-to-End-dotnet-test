using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Contracts.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Database.Repositories
{
    public class MongoDbItemRepository : IItemRepository
    {
        private const string DatabaseName = "catalog";
        private const string CollectionName = "items";
        private readonly IMongoCollection<Item> _itemsCollection;
        private readonly FilterDefinitionBuilder<Item> _filterBuilder = Builders<Item>.Filter;

        public MongoDbItemRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(DatabaseName);
            _itemsCollection = database.GetCollection<Item>(CollectionName);
        }
        public async Task<Item> GetItemAsync(string id)
        {
            var filter = _filterBuilder.Eq(item => item.Id, id);
            return await _itemsCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            return await _itemsCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task CreateItemAsync(Item item)
        {
            await _itemsCollection.InsertOneAsync(item);
        }

        public async Task UpdateItemAsync(Item item)
        {
            var filter = _filterBuilder.Eq(existingItem => existingItem.Id, item.Id);
            await _itemsCollection.ReplaceOneAsync(filter, item);
        }

        public async Task DeleteItemAsync(string id)
        {
            var filter = _filterBuilder.Eq(item => item.Id, id);
            await _itemsCollection.DeleteOneAsync(filter);
        }
    }
}
