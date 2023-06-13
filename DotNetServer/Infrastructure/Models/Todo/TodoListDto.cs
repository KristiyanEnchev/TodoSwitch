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
        public int OrderIndex { get; set; }
        public string? Icon { get; init; }

        public IReadOnlyCollection<TodoItemDto> TodoItems { get; set; }

        public virtual void Mapping(Profile mapper)
        {
            mapper.CreateMap<TodoList, TodoListDto>()
                .ForMember(dest => dest.TodoItems, opt => opt.MapFrom(src => src.TodoItems))
                .AfterMap((src, dest) => dest.TodoItems = dest.TodoItems.OrderBy(x => x.OrderIndex).ToList());
        }
    }
}