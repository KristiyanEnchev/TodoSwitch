namespace Infrastructure.Services.Identity
{
    using System.Text;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.WebUtilities;

    using Domain.Events;
    using Domain.Entities.Identity;

    using Shared;
    using Shared.Exceptions;

    using Application.Interfaces.Services;
    using Domain.Entities;
    using Infrastructure.Services.Helpers;
    using Amazon.Util.Internal.PlatformServices;
    using AspNetCore.Identity.MongoDbCore.Models;
    using Infrastructure.Services.Token;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;

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
                CreatedDate = DateTime.UtcNow,
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

            string ipAddress = IpHelper.GetIpAddress();

            var tokenResult = await jwtGenerator.GenerateTokenAsync(user, ipAddress);

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