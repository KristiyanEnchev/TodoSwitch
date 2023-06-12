namespace Domain.Common
{
    using System.Runtime.Serialization;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    using Domain.Common.Interfaces;

    public abstract class BaseEntity : IEntity
    {
        protected BaseEntity()
        {
            this.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        }

        [BsonId]
        [DataMember]
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}