namespace Application.Interfaces.Services
{
    using Domain.Entities;

    using Models.Todo;

    using Shared;

    public interface ICachedTodoService
    {
        Task<List<SupportedColorDto>> GetAllSupportedListColors();
        Task<Result<List<GetUserTodoListsDto>>> GetUserTodoListsAsync(string userId);
        Task<PaginatedResult<TodoItemDto>> GetTodoItemsForListAsync(string userId, string todoListId, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<Result<GetUserTodoItemDto>> GetTodoItemByIdAsync(string userId, string listId, string itemId);
        Task<Result<TodoItemDto>> ToggleTodoItemDoneStatusAsync(string userId, string todoListId, string todoItemId);
        Task<Result<TodoItemDto>> UpdateTodoItemPriorityAsync(string userId, string todoListId, string todoItemId, PriorityLevel newPriority);
        Task<Result<TodoItemDto>> UpdateTodoItemOrderIndexAsync(string userId, string todoListId, string todoItemId, int newOrderIndex);
        Task<Result<TodoItemDto>> CreateTodoItemAsync(string userId, CreateTodoItemDto todoItemDto);
        Task<Result<string>> DeleteTodoItemAsync(string userId, string todoListId, string todoItemId);
        Task<Result<TodoItemDto>> UpdateTodoItemDescriptionAsync(string userId, string todoListId, string itemId, string title, string note);
        Task<Result<TodoListDto>> CreateTodoListAsync(string userId, string title, string colorCode);
        Task<Result<string>> DeleteTodoListAsync(string userId, string todoListId);
        Task<Result<TodoListDto>> UpdateTodoListTitleAsync(string userId, string listId, string title);
        Task<Result<TodoListDto>> UpdateTodoListColorAsync(string userId, string todoListId, string colorCode);
    }
}