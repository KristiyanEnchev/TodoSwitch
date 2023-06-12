namespace Application.Handlers.TodoLists.Queries.GetTodos
{
    using Microsoft.AspNetCore.Authorization;

    using MediatR;

    using Application.Interfaces.Services;

    using Shared;

    using Models.Todo;

    [Authorize]
    public record GetPagedTodosForUSerQuery : IRequest<PaginatedResult<TodoListDto>>
    {
        public string? UserId { get; set; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public bool Order { get; }

        public GetPagedTodosForUSerQuery(int pageNumber, int pageSize, bool order, string userId)
        {
            UserId = userId;
            PageNumber = pageNumber;
            PageSize = pageSize;
            Order = order;
        }

        public class GetPagedTodosQueryHandler : IRequestHandler<GetPagedTodosForUSerQuery, PaginatedResult<TodoListDto>>
        {
            private readonly ITodoService _todoService;

            public GetPagedTodosQueryHandler(ITodoService todoService)
            {
                _todoService = todoService;
            }

            public async Task<PaginatedResult<TodoListDto>> Handle(GetPagedTodosForUSerQuery request, CancellationToken cancellationToken)
            {
                var result = await _todoService.GetPagedTodosForUser(request.Order, request.PageNumber, request.PageSize, request.UserId!, cancellationToken);

                return result;
            }
        }
    }
}