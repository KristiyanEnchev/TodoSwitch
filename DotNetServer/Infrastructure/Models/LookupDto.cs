namespace Models
{
    using Domain.Entities;

    using Shared.Mappings;

    public class LookupDto : IMapFrom<TodoList>, IMapFrom<TodoItem>
    {
        public int Id { get; init; }

        public string? Title { get; init; }
    }
}