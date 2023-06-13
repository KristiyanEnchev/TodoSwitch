namespace Application.Handlers.Identity.Common
{
    public class UserResponseModel
    {
        public UserResponseModel(string token, DateTime refreshTokenExpiryTime, string refreshToken)
        {
            AccessToken = token;
            RefreshTokenExpiryTime = refreshTokenExpiryTime;
            RefreshToken = refreshToken;
        }

        public string AccessToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string RefreshToken { get; set; }
    }
}