using Guardian.Domain.User.Entities;
using System.Text.Json.Serialization;

namespace Guardian.Application.Accounts.Dtos.Outputs
{
    public class AuthenticationResponse
    {
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public bool IsActive { get; set; }
        public bool IsFirstLogin { get; set; }

        [JsonIgnore]
        public string AccessToken { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }

        public static AuthenticationResponse Map(ApplicationUser user, IEnumerable<string> roles, string accessToken, string refreshToken)
        {
            return new()
            {
                Email = user.Email,
                IsActive = user.IsActive,
                Roles = roles,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                IsFirstLogin = user.IsDefaultPassword || user.LastLogin is null
            };
        }
    }
}
