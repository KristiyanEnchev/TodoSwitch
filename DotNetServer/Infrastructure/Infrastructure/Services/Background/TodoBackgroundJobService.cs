namespace Infrastructure.Services.Background
{
    using System.Threading.Tasks;

    using Application.Interfaces.Services;

    using Domain.Entities;

    using Persistence.Repository;

    public class TodoBackgroundJobService : ITodoBackgroundJobService
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IMongoRepository<TodoList> _todoListRepo;
        private readonly IMongoRepository<TodoItem> _todoItemRepo;

        public TodoBackgroundJobService(IBackgroundTaskQueue taskQueue, IMongoRepository<TodoList> todoListRepository, IMongoRepository<TodoItem> todoItemRepository)
        {
            _taskQueue = taskQueue;
            _todoListRepo = todoListRepository;
            _todoItemRepo = todoItemRepository;
        }

        public Task QueueUpdateTodoItemAsync(string todoItemId, Action<TodoItem> updateAction)
        {
            _taskQueue.QueueBackgroundWorkItem(async token =>
            {
                var todoItem = await _todoItemRepo.GetByIdAsync(todoItemId);
                if (todoItem != null)
                {
                    updateAction(todoItem);
                    await _todoItemRepo.UpdateAsync(todoItem);
                }
            });

            return Task.CompletedTask;
        }

        public Task QueueDeleteTodoItemAsync(string todoItemId)
        {
            _taskQueue.QueueBackgroundWorkItem(async token =>
            {
                var todoItem = await _todoItemRepo.GetByIdAsync(todoItemId);
                if (todoItem != null)
                {
                    await _todoItemRepo.DeleteAsync(todoItemId);
                }
            });

            return Task.CompletedTask;
        }

        public Task QueueUpdateTodoListAsync(string todoListId, Action<TodoList> updateAction)
        {
            _taskQueue.QueueBackgroundWorkItem(async token =>
            {
                var todoList = await _todoListRepo.GetByIdAsync(todoListId);
                if (todoList != null)
                {
                    updateAction(todoList);
                    await _todoListRepo.UpdateAsync(todoList);
                }
            });

            return Task.CompletedTask;
        }

        public Task QueueDeleteTodoListAsync(string todoListId)
        {
            _taskQueue.QueueBackgroundWorkItem(async token =>
            {
                var todoList = await _todoListRepo.GetByIdAsync(todoListId);
                if (todoList != null)
                {
                    foreach (var todoItem in todoList.TodoItems)
                    {
                        await QueueDeleteTodoItemAsync(todoItem.Id);
                    }

                    await _todoListRepo.DeleteAsync(todoListId);
                }
            });

            return Task.CompletedTask;
        }

        public Task QueueCreateTodoListAsync(TodoList todoList)
        {
            _taskQueue.QueueBackgroundWorkItem(async token =>
            {
                await _todoListRepo.AddAsync(todoList);
            });

            return Task.CompletedTask;
        }

        public Task QueueCreateTodoItemAsync(TodoItem todoItem)
        {
            _taskQueue.QueueBackgroundWorkItem(async token =>
            {
                await _todoItemRepo.AddAsync(todoItem);
            });

            return Task.CompletedTask;
        }
    }
}