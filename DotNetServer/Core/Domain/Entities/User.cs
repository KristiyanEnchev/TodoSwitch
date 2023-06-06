namespace Domain.Entities.Identity
{
    using AspNetCore.Identity.MongoDbCore.Models;

    using MongoDbGenericRepository.Attributes;

    using Domain.Common.Interfaces;

    [CollectionName("user")]
    public class User : MongoIdentityUser<string>, IAuditableEntity
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsActive { get; set; }

        public string? CreatedBy { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
    }
}