namespace Domain.Entities
{
    using MongoDB.Bson.Serialization.Attributes;

    using Domain.Common;
    using Domain.Attributes;

    [BsonCollection("todolists")]
    public class TodoList : BaseAuditableEntity
    {
        public string UserId { get; set; }

        [BsonElement("Title")]
        public string? Title { get; set; }
        public Color Color { get; set; } = Color.White;

        [BsonElement("TodoItems")]
        public IList<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
    }
}