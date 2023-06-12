namespace Models.Todo
{
    using AutoMapper;

    using Domain.Entities;

    using Shared.Mappings;

    public class TodoListDto : IMapFrom<TodoList>
    {
        public TodoListDto()
        {
            TodoItems = new HashSet<TodoItemDto>();
        }

        public string? Id { get; init; }
        public string? Title { get; init; }
        public string? Color { get; init; }
        public IReadOnlyCollection<TodoItemDto> TodoItems { get; set; }

        public virtual void Mapping(Profile mapper)
        {
            mapper.CreateMap<TodoList, TodoListDto>()
                .AfterMap((src, dest) => dest.TodoItems = dest.TodoItems.OrderBy(x => x.OrderIndex).ToList());

            mapper.CreateMap<TodoListDto, TodoList>()
                .AfterMap((src, dest) => dest.TodoItems = dest.TodoItems.OrderBy(x => x.OrderIndex).ToList());
        }
    }
}