using System.Text.Json.Serialization;

namespace Guardian.Application.Accounts.Dtos.Outputs
{
    public class RefreshTokenResponse
    {
        [JsonIgnore]
        public string AccessToken { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}
