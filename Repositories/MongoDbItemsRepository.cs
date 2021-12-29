using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Repositories
{
    public class MongoDbItemsRepository : IItemsRepository
    {
        private const string databaseName = "catalog";
        private const string collectionName = "items";

        private readonly IMongoCollection<Item> _itemsCollection;
        private readonly FilterDefinitionBuilder<Item> _filterBuilder = Builders<Item>.Filter;

        public MongoDbItemsRepository(IMongoClient client)
        {
            if (client is null)
                throw new ArgumentNullException(nameof(client));

            var database = client.GetDatabase(databaseName);
            _itemsCollection = database.GetCollection<Item>(collectionName);
        }

        public async Task<IEnumerable<Item>> GetItemsAsync() {

         return await _itemsCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<Item> GetItemAsync(Guid id)
        {
            var filter = _filterBuilder.Eq(item => item.Id, id);
            return await _itemsCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task CreateItemAsync(Item item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            await _itemsCollection.InsertOneAsync(item);
        }

        public async Task DeleteItem(Guid id)
        {
            var filter = _filterBuilder.Eq(item => item.Id, id);
            await _itemsCollection.DeleteOneAsync(filter);
        }        

        public async Task UpdateItemAsync(Item item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var filter = _filterBuilder.Eq(existingItem => existingItem.Id, item.Id);
            await _itemsCollection.ReplaceOneAsync(filter, item);
        }
    }
}