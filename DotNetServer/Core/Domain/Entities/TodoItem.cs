namespace Domain.Entities
{
    using Domain.Attributes;
    using Domain.Common;

    [BsonCollection("TodoItem")]

    public class TodoItem : BaseAuditableEntity
    {
        public string? ListId { get; set; }

        public string? Title { get; set; }

        public string? Note { get; set; }

        public int OrderIndex { get; set; }

        public PriorityLevel Priority { get; set; }

        public DateTime? Reminder { get; set; }

        private bool _done;
        public bool Done
        {
            get => _done;
            set
            {
                _done = value;
            }
        }

        public TodoList List { get; set; } = null!;
    }
}