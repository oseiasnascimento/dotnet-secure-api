using Guardian.Application.Accounts.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Identity.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SymmetricSecurityKey AuthSigningKey()
        {
            string signatureKey = _configuration["JWTSettings:Key"];
            SymmetricSecurityKey authSigningKey = new(Encoding.UTF8.GetBytes(signatureKey));
            return authSigningKey;
        }

        public virtual JwtSecurityToken GenerateAccessToken(List<Claim> claims)
        {
            _ = int.TryParse(_configuration["JWTSettings:AccessTokenValidityInMinutes"],
                             out int accessTokenValidityInMinutes);

            JwtSecurityToken jwtSecurityToken = new(
                claims: claims,
                expires: DateTime.Now.AddMinutes(accessTokenValidityInMinutes),
                signingCredentials: new SigningCredentials(AuthSigningKey(), SecurityAlgorithms.HmacSha256),
                issuer: _configuration["JWTSettings:Issuer"],
                audience: _configuration["JWTSettings:Audience"]
            );

            return jwtSecurityToken;
        }

        public virtual string GenerateRefreshToken()
        {
            using RandomNumberGenerator generator = RandomNumberGenerator.Create();
            var randomBytes = new byte[40];
            generator.GetBytes(randomBytes);
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        public ClaimsPrincipal GetPrincipalForExpiredToken(string token)
        {
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = AuthSigningKey(),
                ValidIssuer = _configuration["JWTSettings:Issuer"],
                ValidAudience = _configuration["JWTSettings:Audience"]
            };

            JwtSecurityTokenHandler tokenHandler = new();
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters,
                out SecurityToken securityToken);

            // Valida se o token é do tipo certo (JwtSecurityToken) e se o Algoritmo usado para criptografar o algoritmo é um HmacSha256
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Token inválido/expirado.");
            }

            return principal;
        }

        public void SetTokensToCookies(string accessToken, string refreshToken, HttpContext context)
        {
            _ = int.TryParse(_configuration["JWTSettings:RefreshTokenValidityInDays"],
                out int tokenValidityInDays);

            // Adicionando expiração em dias para os tokens
            // Essa expiração serve para REMOVER os tokens dos cookies, forçando o usuário a realizar autenticação novamente 

            context.Response.Cookies.Append("access_token", accessToken,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(tokenValidityInDays),
                    HttpOnly = true,
                    IsEssential = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });

            context.Response.Cookies.Append("refresh_token", refreshToken,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(tokenValidityInDays),
                    HttpOnly = true,
                    IsEssential = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });
        }
    }
}