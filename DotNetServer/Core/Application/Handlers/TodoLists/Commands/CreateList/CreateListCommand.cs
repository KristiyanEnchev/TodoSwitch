namespace Application.Handlers.TodoLists.Commands.CreateList
{
    using MediatR;

    using Models.Todo;

    using Application.Interfaces.Services;

    using Shared;

    public class CreateListCommand : IRequest<Result<TodoListDto>>
    {
        public string UserId { get; init; }
        public string Title { get; init; }
        public string Icon { get; init; }
        public string ColorCode { get; init; }

        public CreateListCommand(string userId, string title, string icon, string colorCode)
        {
            UserId = userId;
            Title = title;
            Icon = icon;
            ColorCode = colorCode;
        }

        public class CreateListCommandHandler : IRequestHandler<CreateListCommand, Result<TodoListDto>> 
        {
            private readonly ICachedTodoService _service;

            public CreateListCommandHandler(ICachedTodoService service)
            {
                _service = service;
            }

            public async Task<Result<TodoListDto>> Handle(CreateListCommand request, CancellationToken cancellationToken) 
            {
                var result = await _service.CreateTodoListAsync(request.UserId, request.Title,request.Icon, request.ColorCode);

                return result;
            } 
        }
    }
}