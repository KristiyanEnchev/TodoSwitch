namespace Application.Common.Behaviours
{
    using System.Diagnostics;

    using Microsoft.Extensions.Logging;

    using MediatR;

    using Application.Interfaces;
    using Application.Handlers.Identity.Commands.Login;

    public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly Stopwatch _timer;
        private readonly ILogger<TRequest> _logger;
        private readonly IUser _user;

        public PerformanceBehaviour(ILogger<TRequest> logger, IUser user)
        {
            _logger = logger;
            _user = user;
            _timer = new Stopwatch();
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                var requestName = typeof(TRequest).Name;
                var userId = _user.Id ?? string.Empty;
                string? email = _user.Email;
                var sanitizedRequest = SanitizeRequest(request);

                _logger.LogWarning("Todo Service Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@Email} {@Request}",
                    requestName, elapsedMilliseconds, userId, email, sanitizedRequest);
            }

            return response;
        }

        private object SanitizeRequest(TRequest request)
        {
            if (request is UserLoginCommand loginRequest)
            {
                var sanitizedEmail = SanitizeEmail(loginRequest.Email);

                var sanitizedPassword = "******";

                var sanitizedRequest = new
                {
                    Email = sanitizedEmail,
                    Password = sanitizedPassword,
                };
                return sanitizedRequest;
            }

            return request;
        }

        private string SanitizeEmail(string email)
        {
            int atIndex = email.IndexOf('@');
            if (atIndex > 0)
            {
                string domainPart = email.Substring(atIndex); 
                string firstTwoLetters = email.Substring(0, Math.Min(2, atIndex)); 

                int lengthToMask = atIndex - firstTwoLetters.Length;
                string maskedCharacters = new string('*', lengthToMask);

                return $"{firstTwoLetters}{maskedCharacters}{domainPart}";
            }
            else
            {
                return email;
            }
        }
    }
}