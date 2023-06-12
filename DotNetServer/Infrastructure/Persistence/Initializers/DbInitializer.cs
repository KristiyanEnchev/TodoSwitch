namespace Persistence.Initializers
{
    using MongoDB.Bson;
    using MongoDB.Driver;

    public class DbInitializer : IDbInitializer
    {
        private readonly IMongoDatabase _database;

        public DbInitializer(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task Initialize()
        {
            await CreateIndexes();
            await CreateCollections();
        }

        private async Task CreateCollections()
        {
            var collectionList = _database.ListCollectionNames().ToList();
            if (!collectionList.Contains("users"))
            {
                _database.CreateCollection("users");
            }
        }

        private async Task CreateIndexes()
        {
            var usersCollection = _database.GetCollection<BsonDocument>("users");
            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexKeys = Builders<BsonDocument>.IndexKeys.Ascending("Email");
            var indexModel = new CreateIndexModel<BsonDocument>(indexKeys, indexOptions);
            usersCollection.Indexes.CreateOne(indexModel);
        }
    }
}
