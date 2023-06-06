namespace Domain.Collections
{
    using System.Runtime.Serialization;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public abstract class MongoBaseDocument
    {
        [BsonId]
        [DataMember]
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public virtual string? _id { get; set; }
    }
}