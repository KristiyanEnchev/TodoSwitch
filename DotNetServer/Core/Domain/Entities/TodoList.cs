namespace Domain.Entities
{
    using Domain.Attributes;
    using Domain.Common;

    [BsonCollection("TodoList")]
    public class TodoList : BaseAuditableEntity
    {
        public string? UserId { get; set; }
        public string? Title { get; set; }
        public Colour Colour { get; set; } = Colour.White;

        public IList<TodoItem> Items { get; set; } = new List<TodoItem>();
    }
}