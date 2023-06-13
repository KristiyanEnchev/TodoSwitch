namespace Domain.Entities
{
    using MongoDB.Bson.Serialization.Attributes;

    using Domain.Common;
    using Domain.Attributes;

    [BsonCollection("todoitems")]

    public class TodoItem : BaseAuditableEntity
    {
        public string ListId { get; set; }

        [BsonElement("Title")]
        public string? Title { get; set; }

        [BsonElement("Note")]
        public string? Note { get; set; }

        [BsonElement("OrderIndex")]
        public int OrderIndex { get; set; }

        [BsonElement("Priority")]
        public PriorityLevel Priority { get; set; }

        public DateTime Reminder { get; set; }

        [BsonElement("IsDone")]
        public bool IsDone { get; set; }
    }
}