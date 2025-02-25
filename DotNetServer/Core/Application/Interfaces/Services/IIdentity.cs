﻿namespace Application.Interfaces.Services
{
    using System.Threading.Tasks;

    using Application.Handlers.Identity.Common;

    using Shared;

    public interface IIdentity
    {
        Task<Result<string>> Register(UserRegisterRequestModel userRequest);
        Task<Result<UserResponseModel>> Login(UserRequestModel userRequest);
        Task<Result<UserResponseModel>> RefreshTokenAsync(UserRefreshModel request);
        Task<Result<string>> LogoutAsync(string userEmail);
    }
}