namespace Application.Handlers.TodoLists.Commands.UpdateTodoList
{
    using MediatR;

    using Models.Todo;

    using Application.Interfaces.Services;

    using Shared;

    public record UpdateListCommand : IRequest<Result<TodoListDto>>
    {
        public string UserId { get; init; }
        public string ListId { get; init; }
        public string Title { get; init; }
        public string Icon { get; init; }
        public string ColorCode { get; init; }

        public UpdateListCommand(string userId, string listId, string title, string icon, string colorCode)
        {
            UserId = userId;
            ListId = listId;
            Title = title;
            Icon = icon;
            ColorCode = colorCode;
        }

        public class UpdateListCommandHandler : IRequestHandler<UpdateListCommand, Result<TodoListDto>>
        {
            private readonly ICachedTodoService _todoService;

            public UpdateListCommandHandler(ICachedTodoService todoService)
            {
                _todoService = todoService;
            }

            public async Task<Result<TodoListDto>> Handle(UpdateListCommand request, CancellationToken cancellationToken)
            {
                var result = await _todoService.UpdateTodoListAsync(request.UserId, request.ListId, request.Title, request.Icon, request.ColorCode);

                return result;
            }
        }
    }
}