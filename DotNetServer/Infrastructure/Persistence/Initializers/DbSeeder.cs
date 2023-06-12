namespace Persistence.Initializers
{
    using Microsoft.AspNetCore.Identity;

    using MongoDB.Bson;
    using MongoDB.Driver;

    using Domain.Entities;

    public class DbSeeder : IDbSeeder
    {
        private readonly IMongoDatabase _database;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<UserRole> _roleManager;

        public DbSeeder(IMongoDatabase database, UserManager<User> userManager, RoleManager<UserRole> roleManager)
        {
            _database = database;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedData()
        {
            var usersCollection = _database.GetCollection<User>("users");
            if (usersCollection.Find(_ => true).Limit(1).CountDocuments() == 0)
            {
                await TrySeedUsersAsync();
            }

            var users = await usersCollection.Find(_ => true).ToListAsync();
            if (!users.Any(u => u.TodoLists.Any()))
            {
                await SeedTodoListsAsync(users);
            }
        }

        private async Task SeedTodoListsAsync(List<User> users)
        {
            var todoListCollection = _database.GetCollection<TodoList>("todolists");
            var todoItemCollection = _database.GetCollection<TodoItem>("todoitems");

            var random = new Random();
            var colors = Color.SupportedColors.ToList();
            var priorities = Enum.GetValues(typeof(PriorityLevel)).Cast<PriorityLevel>().ToList();

            foreach (var user in users)
            {
                for (int i = 0; i < 3; i++)
                {
                    var todoList = new TodoList
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        UserId = user.Id,
                        Title = $"Task List {i + 1}",
                        Color = colors[random.Next(colors.Count)],
                        CreatedBy = "Seeder",
                        CreatedOn = DateTime.UtcNow,
                        TodoItems = new List<TodoItem>()
                    };

                    int itemsCount = random.Next(5, 15);
                    for (int j = 0; j < itemsCount; j++)
                    {
                        var todoItem = new TodoItem
                        {
                            Id = ObjectId.GenerateNewId().ToString(),
                            ListId = todoList.Id,
                            Title = $"Task {j + 1}",
                            Note = "This is a seeded task",
                            Priority = priorities[random.Next(priorities.Count)],
                            Reminder = DateTime.UtcNow.AddDays(random.Next(1, 30)),
                            IsDone = random.NextDouble() >= 0.5,
                            OrderIndex = j,
                            CreatedBy = "Seeder",
                            CreatedOn = DateTime.UtcNow,
                        };
                        todoList.TodoItems.Add(todoItem);

                        await todoItemCollection.InsertOneAsync(todoItem);
                    }
                    user.TodoLists.Add(todoList);

                    await todoListCollection.InsertOneAsync(todoList);
                }
                var usersCollection = _database.GetCollection<User>("users");
                await usersCollection.ReplaceOneAsync(u => u.Id == user.Id, user);
            }
        }

        public async Task TrySeedUsersAsync()
        {
            var administratorRole = new UserRole { Name = "Administrator", CreatedBy = "Seeder", CreatedOn = DateTime.UtcNow };
            var userRole = new UserRole { Name = "User", CreatedBy = "Seeder", CreatedOn = DateTime.UtcNow };

            if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
            {
                await _roleManager.CreateAsync(administratorRole);
            }

            if (_roleManager.Roles.All(r => r.Name != userRole.Name))
            {
                await _roleManager.CreateAsync(userRole);
            }

            var administrator = new User
            {
                Id = "9d0155b3-64d0-4d30-b802-0c2bd1086296",
                UserName = "admin@admin.com",
                FirstName = "Super",
                LastName = "Admin",
                Email = "admin@admin.com",
                IsActive = true,
                CreatedBy = "Initial Seed",
                EmailConfirmed = true,
                TodoLists = new List<TodoList>()
            };

            if (_userManager.Users.All(u => u.UserName != administrator.UserName))
            {
                await _userManager.CreateAsync(administrator, "123456");
                if (!string.IsNullOrWhiteSpace(administratorRole.Name))
                {
                    await _userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
                }
            }

            var user1 = new User
            {
                Id = "58efac42-e31d-4039-bfe1-76672c615dd5",
                UserName = "user1@example.com",
                Email = "user1@example.com",
                FirstName = "User",
                LastName = "One",
                IsActive = true,
                CreatedBy = "Initial Seed",
                EmailConfirmed = true,
                TodoLists = new List<TodoList>()
            };
            var user2 = new User
            {
                Id = "8e9ce019-4ef0-45bc-8ec1-bc2ced9b1e16",
                UserName = "user2@example.com",
                Email = "user2@example.com",
                FirstName = "User",
                LastName = "Two",
                IsActive = true,
                CreatedBy = "Initial Seed",
                EmailConfirmed = true,
                TodoLists = new List<TodoList>()
            };

            var some = await _userManager.CreateAsync(user1, "123456");
            if (!string.IsNullOrWhiteSpace(userRole.Name))
            {
                await _userManager.AddToRolesAsync(user1, new[] { userRole.Name });
            }

            await _userManager.CreateAsync(user2, "123456");
            if (!string.IsNullOrWhiteSpace(userRole.Name))
            {
                await _userManager.AddToRolesAsync(user2, new[] { userRole.Name });
            }
        }
    }
}