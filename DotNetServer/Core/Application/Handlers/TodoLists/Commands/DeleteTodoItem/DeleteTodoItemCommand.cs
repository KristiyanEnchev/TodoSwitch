namespace Application.Handlers.TodoLists.Commands.DeleteTodoItem
{
    using MediatR;

    using Application.Interfaces.Services;

    using Shared;

    public record DeleteTodoItemCommand : IRequest<Result<string>>
    {
        public string UserId { get; init; }
        public string ListId { get; init; }
        public string ItemId { get; }

        public DeleteTodoItemCommand(string userId, string listId, string itemId)
        {
            UserId = userId;
            ListId = listId;
            ItemId = itemId;
        }

        public class DeleteTodoItemCommandHandler : IRequestHandler<DeleteTodoItemCommand, Result<string>> 
        {
            private readonly ICachedTodoService _service;

            public DeleteTodoItemCommandHandler(ICachedTodoService service)
            {
                _service = service;
            }

            public async Task<Result<string>> Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken) 
            {
                var result = await _service.DeleteTodoItemAsync(request.UserId, request.ListId, request.ItemId);

                return result;
            }
        }
    }
}