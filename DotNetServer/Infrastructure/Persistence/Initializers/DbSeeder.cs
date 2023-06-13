namespace Persistence.Initializers
{
    using Microsoft.AspNetCore.Identity;

    using MongoDB.Bson;
    using MongoDB.Driver;

    using Domain.Entities;
    using Application.Interfaces.Services;
    using System.Threading.Tasks;

    public class DbSeeder : IDbSeeder
    {
        private readonly IMongoDatabase _database;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<UserRole> _roleManager;
        private readonly IOrderService _orderService;

        public DbSeeder(IMongoDatabase database, UserManager<User> userManager, RoleManager<UserRole> roleManager, IOrderService orderService)
        {
            _database = database;
            _userManager = userManager;
            _roleManager = roleManager;
            _orderService = orderService;
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

            var allTasks = new Dictionary<string, List<TodoItem>>
            {
                ["Work"] = new List<TodoItem>
                {
                    new TodoItem { Title = "Complete project proposal", Note = "Finish the draft and send for review", IsDone = false, OrderIndex = 0 },
                    new TodoItem { Title = "Review team performance", Note = "Prepare feedback for quarterly review", IsDone = false, OrderIndex = 1 },
                    new TodoItem { Title = "Update website content", Note = "Refresh the about page and add new testimonials", IsDone = true, OrderIndex = 2 },
                    new TodoItem { Title = "Plan team building event", Note = "Research venues and activities for next month", IsDone = false, OrderIndex = 3 },
                    new TodoItem { Title = "Create project timeline", Note = "Establish deadlines for project milestones", IsDone = false, OrderIndex = 4 },
                    new TodoItem { Title = "Schedule client meeting", Note = "Discuss project scope and deliverables", IsDone = false, OrderIndex = 5 },
                    new TodoItem { Title = "Design project architecture", Note = "Draft the system design diagram", IsDone = false, OrderIndex = 6 },
                },
                ["Personal"] = new List<TodoItem>
                {
                    new TodoItem { Title = "Go for a run", Note = "30 minutes in the park", IsDone = false, OrderIndex = 0 },
                    new TodoItem { Title = "Read a book", Note = "Finish chapter 5 of 'The Great Gatsby'", IsDone = true, OrderIndex = 1 },
                    new TodoItem { Title = "Call mom", Note = "It's her birthday tomorrow", IsDone = false, OrderIndex = 2 },
                    new TodoItem { Title = "Meditate", Note = "10 minutes of mindfulness", IsDone = false, OrderIndex = 3 },
                    new TodoItem { Title = "Buy groceries", Note = "Milk, bread, eggs, and vegetables", IsDone = false, OrderIndex = 4 },
                    new TodoItem { Title = "Write in journal", Note = "Reflect on the day", IsDone = false, OrderIndex = 5 },
                    new TodoItem { Title = "Watch a movie", Note = "Enjoy a relaxing evening", IsDone = false, OrderIndex = 6 },
                },
                ["Home"] = new List<TodoItem>
                {
                    new TodoItem { Title = "Clean the garage", Note = "Organize tools and dispose of old items", IsDone = false, OrderIndex = 0 },
                    new TodoItem { Title = "Water the plants", Note = "Don't forget the ones on the balcony", IsDone = false, OrderIndex = 1 },
                    new TodoItem { Title = "Repair leaky faucet", Note = "Fix the kitchen sink", IsDone = false, OrderIndex = 2 },
                    new TodoItem { Title = "Paint the fence", Note = "Apply a fresh coat of paint", IsDone = false, OrderIndex = 3 },
                    new TodoItem { Title = "Declutter the attic", Note = "Sort through old boxes", IsDone = false, OrderIndex = 4 },
                    new TodoItem { Title = "Mow the lawn", Note = "Trim the grass in the backyard", IsDone = false, OrderIndex = 5 },
                    new TodoItem { Title = "Replace light bulbs", Note = "Check and replace burnt-out bulbs", IsDone = false, OrderIndex = 6 },
                },
                ["Education"] = new List<TodoItem>
                {
                    new TodoItem { Title = "Study for exam", Note = "Review chapters 3-5", IsDone = false, OrderIndex = 0 },
                    new TodoItem { Title = "Complete assignment", Note = "Submit the math homework", IsDone = false, OrderIndex = 1 },
                    new TodoItem { Title = "Watch lecture", Note = "Catch up on the recorded lecture", IsDone = false, OrderIndex = 2 },
                    new TodoItem { Title = "Group study", Note = "Meet with classmates for study session", IsDone = false, OrderIndex = 3 },
                    new TodoItem { Title = "Read textbook", Note = "Finish reading chapter 6", IsDone = false, OrderIndex = 4 },
                    new TodoItem { Title = "Practice problems", Note = "Solve the practice exercises", IsDone = false, OrderIndex = 5 },
                    new TodoItem { Title = "Research project", Note = "Start gathering sources for research paper", IsDone = false, OrderIndex = 6 },
                },
                ["Health/Fitness"] = new List<TodoItem>
                {
                    new TodoItem { Title = "Morning Yoga", Note = "30-minute session", IsDone = false, OrderIndex = 0 },
                    new TodoItem { Title = "Evening walk", Note = "Take a 20-minute walk after dinner", IsDone = false, OrderIndex = 1 },
                    new TodoItem { Title = "Meal prep", Note = "Prepare meals for the week", IsDone = false, OrderIndex = 2 },
                    new TodoItem { Title = "Hydrate", Note = "Drink 8 glasses of water", IsDone = false, OrderIndex = 3 },
                    new TodoItem { Title = "Strength training", Note = "15-minute workout", IsDone = false, OrderIndex = 4 },
                    new TodoItem { Title = "Sleep early", Note = "Go to bed by 10 PM", IsDone = false, OrderIndex = 5 },
                    new TodoItem { Title = "Stretching exercises", Note = "10 minutes before bed", IsDone = false, OrderIndex = 6 },
                }
            };

            var listDefinitions = new List<(string Title, string Icon, Color Color, int OrderIndex)>
            {
                ("Work", "1f3e2", Color.Orange, 0),
                ("Personal", "1f464", Color.Purple, 1),
                ("Home", "1f3e0", Color.Yellow, 2),
                ("Education", "1f4da", Color.Blue, 3),
                ("Health/Fitness", "1f4aa", Color.Green, 4)
            };

            var priorities = Enum.GetValues(typeof(PriorityLevel)).Cast<PriorityLevel>().ToList();

            foreach (var user in users)
            {
                foreach (var (title, icon, color, orderIndex) in listDefinitions)
                {
                    var todoList = new TodoList
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        UserId = user.Id,
                        Title = title,
                        Icon = icon,
                        Color = color,
                        CreatedBy = "Seeder",
                        CreatedOn = DateTime.UtcNow,
                        TodoItems = new List<TodoItem>(),
                        OrderIndex = orderIndex
                    };

                    foreach (var task in allTasks[title])
                    {
                        task.Id = ObjectId.GenerateNewId().ToString();
                        task.ListId = todoList.Id;
                        task.CreatedBy = "Seeder";
                        task.CreatedOn = DateTime.UtcNow;
                        task.Priority = priorities[random.Next(priorities.Count)];
                        task.Reminder = DateTime.UtcNow.AddHours(random.Next(-1, 48));

                        todoList.TodoItems.Add(task);
                        await todoItemCollection.InsertOneAsync(task);
                    }

                    int additionalTasks = random.Next(6, 10) - allTasks[title].Count;

                    for (int i = 0; i < additionalTasks; i++)
                    {
                        var todoItem = new TodoItem
                        {
                            Id = ObjectId.GenerateNewId().ToString(),
                            ListId = todoList.Id,
                            Title = $"Additional Task {i + 1} for {title}",
                            Note = "This is an additional randomly generated task",
                            IsDone = random.NextDouble() >= 0.5,
                            OrderIndex = allTasks[title].Count + i,
                            CreatedBy = "Seeder",
                            CreatedOn = DateTime.UtcNow,
                            Priority = priorities[random.Next(priorities.Count)],
                            Reminder = DateTime.UtcNow.AddDays(random.Next(1, 30)),
                        };

                        todoList.TodoItems.Add(todoItem);
                        await todoItemCollection.InsertOneAsync(todoItem);
                    }

                    _orderService.ReorderItems(todoList.TodoItems.ToList(), null, 0);

                    user.TodoLists.Add(todoList);
                    await todoListCollection.InsertOneAsync(todoList);
                }

                _orderService.ReorderLists(user.TodoLists, null, 0);

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