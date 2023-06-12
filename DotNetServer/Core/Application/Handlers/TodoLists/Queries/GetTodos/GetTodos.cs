namespace Application.Handlers.TodoLists.Queries.GetTodos
{
    using Microsoft.AspNetCore.Authorization;

    using MediatR;

    using Application.Interfaces.Services;

    using Models.Todo;

    using Shared;

    [Authorize]
    public record GetTodosQuery : IRequest<Result<TodosVm>>
    {
        public class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, Result<TodosVm>>
        {
            private readonly ITodoService _todoService;

            public GetTodosQueryHandler(ITodoService todoService)
            {
                _todoService = todoService;
            }

            public async Task<Result<TodosVm>> Handle(GetTodosQuery request, CancellationToken cancellationToken)
            {
                var result = await _todoService.GetTodos();

                return result;
            }
        }
    }
}