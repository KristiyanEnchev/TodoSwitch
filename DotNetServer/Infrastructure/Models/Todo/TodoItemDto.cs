namespace Models.Todo
{
    using AutoMapper;

    using Domain.Entities;

    using Shared.Mappings;

    public class TodoItemDto : IMapFrom<TodoItem>
    {
        public string? Id { get; init; }

        public string? ListId { get; init; }

        public string? Title { get; init; }

        public bool Done { get; init; }

        public int OrderIndex { get; init; }

        public int Priority { get; init; }

        public string? Note { get; init; }
        public virtual void Mapping(Profile mapper)
        {
            mapper.CreateMap<TodoItem, TodoItemDto>();
            mapper.CreateMap<TodoItemDto, TodoItem>();
        }
    }
}