namespace Infrastructure.Services.Token
{
    using System.Text;
    using System.Security.Claims;
    using System.IdentityModel.Tokens.Jwt;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    using Domain.Entities;

    using Models;

    using Shared.Exceptions;

    using Application.Interfaces.Services;

    public class JwtService : IJwtService
    {
        private readonly UserManager<User> userManager;
        private readonly TokenSettings tokenSettings;

        public JwtService(UserManager<User> UserManager, IOptions<TokenSettings> TokenSettings)
        {
            userManager = UserManager;
            tokenSettings = TokenSettings.Value;
        }

        public async Task<string> GenerateTokenAsync(User user, string ipAddress)
        {
            var claimTypeFirstName = "FirstName";
            var claimTypeLastName = "LastName";
            var claimTypeEmail = "Email";
            var claimTypeUsername = "Username";
            var claimTypeId = "Id";

            var userRoles = await userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            for (int i = 0; i < userRoles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", userRoles[i]));
            }

            var claims = new List<Claim>
            {
                new Claim(claimTypeId, user.Id.ToString()),
                new Claim(claimTypeEmail, user.Email!),
                new Claim(claimTypeFirstName, user.FirstName!),
                new Claim(claimTypeLastName, user.LastName!),
                new Claim(claimTypeUsername, user.UserName!),
                new Claim("ip", ipAddress)
            }
            .Union(roleClaims);

            var secret = Encoding.UTF8.GetBytes(tokenSettings.Key);

            var token = new JwtSecurityToken(
                issuer: tokenSettings.Issuer,
                audience: tokenSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(tokenSettings.DurationInMinutes),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(secret),
                    SecurityAlgorithms.HmacSha256));

            var tokenHandler = new JwtSecurityTokenHandler();
            var encryptedToken = tokenHandler.WriteToken(token);

            return encryptedToken;
        }

        public async Task<string> GenerateRefreshToken(User user)
        {
            await userManager.RemoveAuthenticationTokenAsync(user, "TodoSwitch", "RefreshToken");
            var newRefreshToken = await userManager.GenerateUserTokenAsync(user, "TodoSwitch", "RefreshToken");
            IdentityResult result = await userManager.SetAuthenticationTokenAsync(user, "TodoSwitch", "RefreshToken", newRefreshToken);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new CustomException($"{string.Join(Environment.NewLine, errors)}");
            }

            return newRefreshToken;
        }
    }
}