namespace Application.Interfaces.Services
{
    using Domain.Entities;

    using Models.Todo;

    using Shared;

    public interface ITodoService
    {
        Task<Result<TodosVm>> GetTodos();
        Task<PaginatedResult<TodoList>> GetPagedTodos(bool ascending, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<PaginatedResult<TodoListDto>> GetPagedTodosForUser(bool ascending, int pageNumber, int pageSize, string userId, CancellationToken cancellationToken);

        Task<TodoItemDto> ToggleDoneTodoItem(string listId, string itemId);
        Task<TodoItemDto> ChangePriorityTodoItem(string listId, string itemId, int newPriority);
    }
}