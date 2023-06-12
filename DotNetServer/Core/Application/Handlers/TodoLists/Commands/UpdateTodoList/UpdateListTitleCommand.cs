namespace Application.Handlers.TodoLists.Commands.UpdateTodoList
{
    using MediatR;

    using Models.Todo;

    using Application.Interfaces.Services;

    using Shared;

    public record UpdateListTitleCommand : IRequest<Result<TodoListDto>>
    {
        public string UserId { get; init; }
        public string ListId { get; init; }
        public string Title { get; init; }

        public UpdateListTitleCommand(string userId, string listId, string title)
        {
            UserId = userId;
            ListId = listId;
            Title = title;
        }

        public class UpdateListTitleCommandHandler : IRequestHandler<UpdateListTitleCommand, Result<TodoListDto>>
        {
            private readonly ICachedTodoService _todoService;

            public UpdateListTitleCommandHandler(ICachedTodoService todoService)
            {
                _todoService = todoService;
            }

            public async Task<Result<TodoListDto>> Handle(UpdateListTitleCommand request, CancellationToken cancellationToken)
            {
                var result = await _todoService.UpdateTodoListTitleAsync(request.UserId, request.ListId, request.Title);

                return result;
            }
        }
    }
}