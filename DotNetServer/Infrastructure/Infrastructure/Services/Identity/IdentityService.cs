namespace Infrastructure.Services.Identity
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;

    using Shared;

    using Application.Interfaces.Services;

    using Domain.Entities;

    using Infrastructure.Services.Helpers;

    using Application.Handlers.Identity.Common;

    internal class IdentityService : IIdentity
    {
        private const string InvalidErrorMessage = "Invalid credentials.";

        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IJwtService jwtGenerator;

        public IdentityService(UserManager<User> userManager, IJwtService jwtGenerator, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.jwtGenerator = jwtGenerator;
            this.signInManager = signInManager;
        }

        public async Task<UserResponseModel> GenerateToken(User user)
        {
            string ipAddress = IpHelper.GetIpAddress();

            var token = await jwtGenerator.GenerateTokenAsync(user, ipAddress);

            var newRefreshToken = await jwtGenerator.GenerateRefreshToken(user);

            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await userManager.UpdateAsync(user);

            var tokenResult = new UserResponseModel(token, user.RefreshTokenExpiryTime, newRefreshToken);

            return tokenResult;
        }

        public async Task<Result<string>> Register(UserRegisterRequestModel userRequest)
        {
            var userExists = await userManager.FindByEmailAsync(userRequest.Email);

            if (userExists != null)
            {
                return Result<string>.Failure("Email already in use.");
            }

            var user = new User()
            {
                FirstName = userRequest.FirstName,
                LastName = userRequest.LastName,
                Email = userRequest.Email,
                UserName = userRequest.Email,
                IsActive = true,
                CreatedBy = "Registration",
            };

            var createUserResult = await userManager.CreateAsync(user, userRequest.Password);
            if (!createUserResult.Succeeded)
            {
                var errors = createUserResult.Errors.Select(e => e.Description).ToList();
                return Result<string>.Failure(errors);
            }

            var addToRoleResult = await userManager.AddToRoleAsync(user, "User");
            if (!addToRoleResult.Succeeded)
            {
                var errors = addToRoleResult.Errors.Select(e => e.Description).ToList();
                return Result<string>.Failure(errors);
            }

            return Result<string>.SuccessResult("Succesfull Registration !");
        }

        public async Task<Result<UserResponseModel>> Login(UserRequestModel userRequest)
        {
            var email = userRequest.Email.Trim().Normalize();
            var user = await userManager.FindByEmailAsync(email);
            if (user == null || !user.IsActive)
            {
                return Result<UserResponseModel>.Failure(new List<string> { InvalidErrorMessage });
            }

            if (!user.EmailConfirmed || await userManager.IsLockedOutAsync(user))
            {
                return Result<UserResponseModel>.Failure(new List<string> { "Account is not accessible at the moment." });
            }

            SignInResult signInResult = await signInManager.PasswordSignInAsync(user, userRequest.Password, false, lockoutOnFailure: true);

            if (!signInResult.Succeeded)
            {
                return Result<UserResponseModel>.Failure(new List<string> { InvalidErrorMessage });
            }

            var tokenResult = await GenerateToken(user);

            return Result<UserResponseModel>.SuccessResult(tokenResult);
        }

        public async Task<Result<UserResponseModel>> RefreshTokenAsync(UserRefreshModel request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return Result<UserResponseModel>.Failure(new List<string> { InvalidErrorMessage });
            }

            string oldRefreshToken = await userManager.GetAuthenticationTokenAsync(user, "TodoSwitch", "RefreshToken");
            bool isValid = await userManager.VerifyUserTokenAsync(user, "TodoSwitch", "RefreshToken", request.RefreshToken);

            if (oldRefreshToken == null || !oldRefreshToken.Equals(request.RefreshToken) || !isValid)
            {
                return Result<UserResponseModel>.Failure(new List<string> { $"Your token is not valid." });
            }

            var tokenResult = await GenerateToken(user);

            await signInManager.SignInAsync(user, false);

            return Result<UserResponseModel>.SuccessResult(tokenResult);
        }

        public async Task<Result<string>> LogoutAsync(string userEmail)
        {
            var user = await userManager.FindByEmailAsync(userEmail);
            if (user != null)
            {
                await userManager.RemoveAuthenticationTokenAsync(user, "TodoSwitch", "RefreshToken");
            }
            await signInManager.SignOutAsync();

            return Result<string>.SuccessResult("Succesfull Logout !");
        }
    }
}