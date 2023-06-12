namespace Domain.Entities
{
    using AspNetCore.Identity.MongoDbCore.Models;

    using MongoDbGenericRepository.Attributes;

    [CollectionName("users")]
    public class User : MongoIdentityUser<string>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsActive { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }


        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}