namespace Application.Interfaces.Services
{
    using Domain.Entities;

    public interface ITodoBackgroundJobService
    {
        Task QueueUpdateTodoItemAsync(string todoItemId, Action<TodoItem> updateAction);
        Task QueueUpdateTodoListAsync(string todoListId, Action<TodoList> updateAction);
        Task QueueDeleteTodoItemAsync(string todoItemId);
        Task QueueDeleteTodoListAsync(string todoListId);
        Task QueueCreateTodoListAsync(TodoList todoList);
        Task QueueCreateTodoItemAsync(TodoItem todoItem);
    }
}