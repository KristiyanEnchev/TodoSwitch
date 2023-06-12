namespace Application.Handlers.TodoLists.Queries.GetTodos
{
    using Microsoft.AspNetCore.Authorization;

    using MediatR;

    using Application.Interfaces.Services;

    using Domain.Entities;

    using Shared;

    [Authorize]
    public record GetPagedTodosQuery : IRequest<PaginatedResult<TodoList>>
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public bool Order { get; }

        public GetPagedTodosQuery(int pageNumber, int pageSize,  bool order)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Order = order;
        }

        public class GetPagedTodosQueryHandler : IRequestHandler<GetPagedTodosQuery, PaginatedResult<TodoList>>
        {
            private readonly ITodoService _todoService;

            public GetPagedTodosQueryHandler(ITodoService todoService)
            {
                _todoService = todoService;
            }

            public async Task<PaginatedResult<TodoList>> Handle(GetPagedTodosQuery request, CancellationToken cancellationToken)
            {
                var result = await _todoService.GetPagedTodos(request.Order, request.PageNumber, request.PageSize, cancellationToken);

                return result;
            }
        }
    }
}