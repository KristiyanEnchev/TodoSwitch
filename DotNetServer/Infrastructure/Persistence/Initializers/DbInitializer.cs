namespace Persistence.Initializers
{
    using MongoDB.Driver;

    using Domain.Entities;

    public class DbInitializer : IDbInitializer
    {
        private readonly IMongoDatabase _database;

        public DbInitializer(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task Initialize()
        {
            await CreateCollectionsAsync();
            await CreateIndexesAsync();
        }

        private async Task CreateCollectionsAsync()
        {
            var collectionNames = await _database.ListCollectionNamesAsync();
            var collections = await collectionNames.ToListAsync();

            if (!collections.Contains("users"))
            {
                await _database.CreateCollectionAsync("users");
            }

            if (!collections.Contains("roles"))
            {
                await _database.CreateCollectionAsync("roles");
            }

            if (!collections.Contains("todolists"))
            {
                await _database.CreateCollectionAsync("todolists");
            }

            if (!collections.Contains("todoitems"))
            {
                await _database.CreateCollectionAsync("todoitems");
            }
        }

        private async Task CreateIndexesAsync()
        {
            var usersCollection = _database.GetCollection<User>("users");
            var rolesCollection = _database.GetCollection<UserRole>("roles");
            var todoListsCollection = _database.GetCollection<TodoList>("todolists");
            var todoItemsCollection = _database.GetCollection<TodoItem>("todoitems");

            var userEmailIndexModel = new CreateIndexModel<User>(
                Builders<User>.IndexKeys.Ascending(u => u.Email),
                new CreateIndexOptions { Unique = true }
            );

            var roleNameIndexModel = new CreateIndexModel<UserRole>(
                Builders<UserRole>.IndexKeys.Ascending(r => r.Name),
                new CreateIndexOptions { Unique = true }
            );

            var todoListUserIdIndexModel = new CreateIndexModel<TodoList>(
                Builders<TodoList>.IndexKeys.Ascending(tl => tl.UserId)
            );

            var todoListOrderIndexModel = new CreateIndexModel<TodoList>(
                Builders<TodoList>.IndexKeys.Ascending(tl => tl.OrderIndex)
            );

            var todoItemListIdIndexModel = new CreateIndexModel<TodoItem>(
                Builders<TodoItem>.IndexKeys.Ascending(ti => ti.ListId)
            );

            var todoItemOrderIndexModel = new CreateIndexModel<TodoItem>(
                Builders<TodoItem>.IndexKeys.Ascending(ti => ti.OrderIndex)
            );

            await usersCollection.Indexes.CreateOneAsync(userEmailIndexModel);
            await rolesCollection.Indexes.CreateOneAsync(roleNameIndexModel);
            await todoListsCollection.Indexes.CreateOneAsync(todoListUserIdIndexModel);
            await todoListsCollection.Indexes.CreateOneAsync(todoListOrderIndexModel);
            await todoItemsCollection.Indexes.CreateOneAsync(todoItemListIdIndexModel);
            await todoItemsCollection.Indexes.CreateOneAsync(todoItemOrderIndexModel);
        }
    }
}