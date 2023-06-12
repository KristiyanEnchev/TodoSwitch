namespace Application.Handlers.TodoLists.Commands.UpdateTodoItem
{
    using MediatR;

    using Application.Interfaces.Services;

    using Models.Todo;

    using Shared;

    public record ToggleTodoStatusCommand : IRequest<Result<TodoItemDto>>
    {
        public string UserId { get; init; }
        public string ListId { get; init; }
        public string ItemId { get; }

        public ToggleTodoStatusCommand(string userId, string listId, string itemId)
        {
            UserId = userId;
            ListId = listId;
            ItemId = itemId;
        }

        public class ToggleTodoStatusCommandHandler : IRequestHandler<ToggleTodoStatusCommand, Result<TodoItemDto>>
        {
            private readonly ICachedTodoService _todoService;

            public ToggleTodoStatusCommandHandler(ICachedTodoService todoService)
            {
                _todoService = todoService;
            }

            public async Task<Result<TodoItemDto>> Handle(ToggleTodoStatusCommand request, CancellationToken cancellationToken)
            {
                var result = await _todoService.ToggleTodoItemDoneStatusAsync(request.UserId, request.ListId, request.ItemId);

                return result;
            }
        }
    }
}