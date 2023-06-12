namespace Models.Todo
{
    using AutoMapper;

    using Domain.Entities;

    using Shared.Mappings;

    public class TodoListDto : IMapFrom<TodoList>
    {
        public TodoListDto()
        {
            Items = Array.Empty<TodoItemDto>();
        }

        public string? Id { get; init; }

        public string? Title { get; init; }

        public string? Colour { get; init; }

        public IReadOnlyCollection<TodoItemDto> Items { get; set; }

        public virtual void Mapping(Profile mapper)
        {
            mapper.CreateMap<TodoList, TodoListDto>()
                .AfterMap((src, dest) => dest.Items = dest.Items.OrderBy(x => x.OrderIndex).ToArray());

            mapper.CreateMap<TodoListDto, TodoList>()
                .AfterMap((src, dest) => dest.Items = dest.Items.OrderBy(x => x.OrderIndex).ToList());
        }
    }
}