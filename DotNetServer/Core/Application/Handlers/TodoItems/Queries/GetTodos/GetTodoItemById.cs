namespace Application.Handlers.TodoItems.Queries.GetTodos
{
    using MediatR;

    using Models.Todo;

    using Application.Interfaces.Services;

    using Shared;

    public record GetTodoItemByIdQuery : IRequest<Result<GetUserTodoItemDto>>
    {
        public string UserId { get; init; }
        public string ListId { get; init; }
        public string ItemId { get; }

        public GetTodoItemByIdQuery(string userId, string listId, string itemId)
        {
            UserId = userId;
            ListId = listId;
            ItemId = itemId;
        }

        public class GetTodoItemByIdQueryHandler : IRequestHandler<GetTodoItemByIdQuery, Result<GetUserTodoItemDto>>
        {
            private readonly ICachedTodoService _todoService;

            public GetTodoItemByIdQueryHandler(ICachedTodoService todoService)
            {
                _todoService = todoService;
            }

            public async Task<Result<GetUserTodoItemDto>> Handle(GetTodoItemByIdQuery request, CancellationToken cancellationToken)
            {
                var result = await _todoService.GetTodoItemByIdAsync(request.UserId, request.ListId, request.ItemId);

                return result;
            }
        }
    }
}