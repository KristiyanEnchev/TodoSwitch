﻿namespace Application.Handlers.Identity.Commands.Refresh
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    using MediatR;

    using Application.Interfaces.Services;
    using Application.Handlers.Identity.Common;

    using Shared;

    public class UserRefreshCommand : UserRefreshModel, IRequest<Result<UserResponseModel>>
    {
        public UserRefreshCommand(string email, string refreshToken)
            : base(email, refreshToken)
        {
        }

        public class UserRefreshCommandHandler : IRequestHandler<UserRefreshCommand, Result<UserResponseModel>>
        {
            private readonly IIdentity identity;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public UserRefreshCommandHandler(IIdentity identity, IHttpContextAccessor httpContextAccessor)
            {
                this.identity = identity;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<Result<UserResponseModel>> Handle(UserRefreshCommand request, CancellationToken cancellationToken)
            {
                var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();

                if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Result<UserResponseModel>.Failure("Authorization token is missing.");
                }

                var result = await identity.RefreshTokenAsync(request);

                return result;
            }
        }
    }
}