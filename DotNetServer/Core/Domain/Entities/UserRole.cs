namespace Domain.Entities
{
    using AspNetCore.Identity.MongoDbCore.Models;

    using Domain.Common.Interfaces;

    using MongoDbGenericRepository.Attributes;

    [CollectionName("roles")]
    public class UserRole : MongoIdentityRole<string>, IAuditableEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }

        public string? CreatedBy { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
    }
}
