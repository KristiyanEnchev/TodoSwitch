namespace Application.Handlers.TodoLists.Commands.UpdateTodoItem
{
    using MediatR;

    using Application.Interfaces.Services;

    using Models.Todo;

    using Domain.Entities;

    using Shared;

    public record ChangePriorityTodoItemCommand : IRequest<Result<TodoItemDto>>
    {
        public string UserId { get; init; }
        public string ListId { get; init; }
        public string ItemId { get; init; }
        public PriorityLevel Priority { get; init; }

        public ChangePriorityTodoItemCommand(string userId, string listId, string itemId, PriorityLevel priority)
        {
            UserId = userId;
            ListId = listId;
            ItemId = itemId;
            Priority = priority;
        }

        public class ChangePriorityTodoItemCommandHandler : IRequestHandler<ChangePriorityTodoItemCommand, Result<TodoItemDto>>
        {
            private readonly ICachedTodoService _todoService;

            public ChangePriorityTodoItemCommandHandler(ICachedTodoService todoService)
            {
                _todoService = todoService;
            }

            public async Task<Result<TodoItemDto>> Handle(ChangePriorityTodoItemCommand request, CancellationToken cancellationToken)
            {
                if (!Enum.IsDefined(typeof(PriorityLevel), request.Priority))
                {
                    throw new ArgumentException($"Invalid priority level: {request.Priority}");
                }

                var result = await _todoService.UpdateTodoItemPriorityAsync(request.UserId, request.ListId, request.ItemId, request.Priority);

                return result;
            }
        }
    }
}
