namespace Application.Handlers.TodoItems.Commands.UpdateTodoItem
{
    using MediatR;

    using Models.Todo;

    using Application.Interfaces.Services;

    using Shared;

    public record UpdateItemsOrderIndexCommand : IRequest<Result<TodoListDto>>
    {
        public string UserId { get; init; }
        public string ListId { get; init; }
        public Dictionary<string, int> ChangedItems { get; }

        public UpdateItemsOrderIndexCommand(string userId, string listId, Dictionary<string, int> changedItems)
        {
            UserId = userId;
            ListId = listId;
            ChangedItems = changedItems;
        }

        public class UpdateItemsOrderIndexCommandHandler : IRequestHandler<UpdateItemsOrderIndexCommand, Result<TodoListDto>>
        {
            private readonly ICachedTodoService _todoService;

            public UpdateItemsOrderIndexCommandHandler(ICachedTodoService todoService)
            {
                _todoService = todoService;
            }

            public async Task<Result<TodoListDto>> Handle(UpdateItemsOrderIndexCommand request, CancellationToken cancellationToken)
            {
                var result = await _todoService.ReorderItems(request.UserId, request.ListId, request.ChangedItems);

                return result;
            }
        }
    }
}