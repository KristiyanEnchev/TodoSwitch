namespace Application.Handlers.TodoItems.Commands.UpdateTodoItem
{
    using MediatR;

    using Models.Todo;

    using Application.Interfaces.Services;

    using Shared;

    using Domain.Entities;

    public record UpdateTodoDescrioptionCommand : IRequest<Result<TodoItemDto>>
    {
        public string UserId { get; init; }
        public string ListId { get; init; }
        public string ItemId { get; init; }
        public string Title { get; init; }
        public string Note { get; init; }
        public PriorityLevel Priority { get; init; }

        public UpdateTodoDescrioptionCommand(string userId, string listId, string itemId, string title, string note, PriorityLevel priority)
        {
            UserId = userId;
            ListId = listId;
            ItemId = itemId;
            Title = title;
            Note = note;
            Priority = priority;
        }

        public class UpdateTodoDescrioptionCommandHandler : IRequestHandler<UpdateTodoDescrioptionCommand, Result<TodoItemDto>>
        {
            private readonly ICachedTodoService _todoService;

            public UpdateTodoDescrioptionCommandHandler(ICachedTodoService todoService)
            {
                _todoService = todoService;
            }

            public async Task<Result<TodoItemDto>> Handle(UpdateTodoDescrioptionCommand request, CancellationToken cancellationToken)
            {
                var result = await _todoService.UpdateTodoItemAsync(request.UserId, request.ListId, request.ItemId, request.Title, request.Note, request.Priority);

                return result;
            }
        }
    }
}