using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Guardian.Application.Accounts.Contracts
{
    public interface ITokenService
    {
        /// <summary>
        /// Gera o acess token de acordo com as claims passadas.
        /// </summary>
        /// <param name="claims">Claims do usuário a ser autenticado.</param>
        /// <returns>Retorna um Token do tipo JWT.</returns>
        public JwtSecurityToken GenerateAccessToken(List<Claim> claims);

        /// <summary>
        /// Gera um Refresh Token.
        /// </summary>
        /// <returns>Retorna o refresh token em formato de string.</returns>
        public string GenerateRefreshToken();

        /// <summary>
        /// Recupera as Claims do token expirado.
        /// </summary>
        /// <param name="token">Access token com todas as Claims atribuídas corretamente.</param>
        /// <returns>Retorna uma lista de Claims.</returns>
        public ClaimsPrincipal GetPrincipalForExpiredToken(string token);

        /// <summary>
        /// Recupera a SignIn Key do sistema.
        /// </summary>
        /// <returns>Retorna uma Key Simétrica criptografada em Bytes.</returns>
        public SymmetricSecurityKey AuthSigningKey();

        /// <summary>
        /// Seta os tokens nos cookies, para aplicar o padrão HttpOnly.
        /// </summary>
        /// <param name="accessToken">Access token que irá para os Cookies.</param>
        /// <param name="refreshToken">Refresh token que irá para os Cookies.</param>
        public void SetTokensToCookies(string accessToken, string refreshToken, HttpContext context);
    }
}