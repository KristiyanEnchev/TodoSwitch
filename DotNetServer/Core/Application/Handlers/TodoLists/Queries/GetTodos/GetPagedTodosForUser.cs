namespace Application.Handlers.TodoLists.Queries.GetTodos
{
    using MediatR;

    using Application.Interfaces.Services;

    using Shared;

    using Models.Todo;

    public record GetPagedTodosForUserQuery : IRequest<PaginatedResult<TodoItemDto>>
    {
        public string? UserId { get; set; }
        public string? ListId { get; set; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public bool Order { get; }

        public GetPagedTodosForUserQuery(int pageNumber, int pageSize, bool order, string userId, string listId)
        {
            ListId = listId;
            UserId = userId;
            PageNumber = pageNumber;
            PageSize = pageSize;
            Order = order;
        }

        public class GetPagedTodosQueryHandler : IRequestHandler<GetPagedTodosForUserQuery, PaginatedResult<TodoItemDto>>
        {
            private readonly ICachedTodoService _todoService;

            public GetPagedTodosQueryHandler(ICachedTodoService todoService)
            {
                _todoService = todoService;
            }

            public async Task<PaginatedResult<TodoItemDto>> Handle(GetPagedTodosForUserQuery request, CancellationToken cancellationToken)
            {
                var result = await _todoService.GetTodoItemsForListAsync(request.UserId, request.ListId, request.PageNumber, request.PageSize, cancellationToken);

                return result;
            }
        }
    }
}