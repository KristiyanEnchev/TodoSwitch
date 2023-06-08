namespace Application.Interfaces.Services
{
    using Domain.Entities;

    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(User user, string ipAddress);

        Task<string> GenerateRefreshToken(User user);
    }
}