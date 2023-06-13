namespace Application.Handlers.TodoLists.Commands.UpdateTodoItem
{
    using MediatR;

    using Models.Todo;

    using Application.Interfaces.Services;

    using Shared;

    public record UpdateTodoItemOrderIndexCommand : IRequest<Result<TodoItemDto>>
    {
        public string UserId { get; init; }
        public string ListId { get; init; }
        public string ItemId { get; init; }
        public int NewOrderIndex { get; }

        public UpdateTodoItemOrderIndexCommand(string userId, string listId, string itemId, int newOrderIndex)
        {
            UserId = userId;
            ListId = listId;
            ItemId = itemId;
            NewOrderIndex = newOrderIndex;
        }

        public class UpdateTodoItemOrderIndexCommandHandler : IRequestHandler<UpdateTodoItemOrderIndexCommand, Result<TodoItemDto>>
        {
            private readonly ICachedTodoService _todoService;

            public UpdateTodoItemOrderIndexCommandHandler(ICachedTodoService todoService)
            {
                _todoService = todoService;
            }

            public async Task<Result<TodoItemDto>> Handle(UpdateTodoItemOrderIndexCommand request, CancellationToken cancellationToken)
            {
                var result = await _todoService.UpdateTodoItemOrderIndexAsync(request.UserId, request.ListId, request.ItemId, request.NewOrderIndex);

                return result;
            }
        }
    }
}