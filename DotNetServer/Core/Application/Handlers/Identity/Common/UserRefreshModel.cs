namespace Application.Handlers.Identity.Common
{
    public abstract class UserRefreshModel
    {
        protected internal UserRefreshModel(string email, string refreshToken)
        {
            Email = email;
            RefreshToken = refreshToken;
        }

        public string Email { get; set; }
        public string RefreshToken { get; set; }
    }
}