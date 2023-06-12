namespace Application.Handlers.TodoLists.Queries.GetList
{
    using MediatR;

    using Application.Interfaces.Services;

    using Models.Todo;

    using Shared;

    public record GetUserTodoListQuery : IRequest<Result<List<GetUserTodoListsDto>>>
    {
        public string UserId { get; init; }

        public GetUserTodoListQuery(string userId)
        {
            UserId = userId;
        }

        public class GetPagedTodosQueryHandler : IRequestHandler<GetUserTodoListQuery, Result<List<GetUserTodoListsDto>>>
        {
            private readonly ICachedTodoService _todoService;

            public GetPagedTodosQueryHandler(ICachedTodoService todoService)
            {
                _todoService = todoService;
            }

            public async Task<Result<List<GetUserTodoListsDto>>> Handle(GetUserTodoListQuery request, CancellationToken cancellationToken)
            {
                var result = await _todoService.GetUserTodoListsAsync(request.UserId);

                return result;
            }
        }
    }
}