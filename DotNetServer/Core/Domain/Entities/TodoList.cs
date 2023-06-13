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
        public Color Color { get; set; } = Color.Yellow;


        [BsonElement("OrderIndex")]
        public int OrderIndex { get; set; }


        [BsonElement("Icon")]
        public string? Icon { get; set; } = "";

        [BsonElement("TodoItems")]
        public IList<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
    }
}