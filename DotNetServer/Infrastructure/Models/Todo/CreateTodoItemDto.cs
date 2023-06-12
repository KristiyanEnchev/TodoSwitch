namespace Models.Todo
{
    using AutoMapper;

    using Domain.Entities;

    using Shared.Mappings;

    public class CreateTodoItemDto : IMapFrom<TodoItem>
    {
        public string ListId { get; set; }
        public string Title { get; init; }
        public string Note { get; init; }
        public PriorityLevel Priority { get; init; }
        public DateTime? Reminder { get; set; }

        public virtual void Mapping(Profile mapper)
        {
            mapper.CreateMap<CreateTodoItemDto, TodoItem>().ReverseMap();
        }
    }
}