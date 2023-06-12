namespace Application.Handlers.TodoLists.Commands.UpdateTodoItem
{
    using MediatR;

    using Models.Todo;

    using Application.Interfaces.Services;

    using Shared;

    public record UpdateTodoDescrioptionCommand : IRequest<Result<TodoItemDto>>
    {
        public string UserId { get; init; }
        public string ListId { get; init; }
        public string ItemId { get; init; }
        public string Title { get; init; }
        public string Note { get; init; }

        public UpdateTodoDescrioptionCommand(string userId, string listId, string itemId, string title, string note)
        {
            UserId = userId;
            ListId = listId;
            ItemId = itemId;
            Title = title;
            Note = note;
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
                var result = await _todoService.UpdateTodoItemDescriptionAsync(request.UserId, request.ListId, request.ItemId, request.Title, request.Note);

                return result;
            }
        }
    }
}