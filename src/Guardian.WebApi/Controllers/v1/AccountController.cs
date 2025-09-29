using Guardian.Application.Accounts.Contracts;
using Guardian.Application.Accounts.Dtos.Inputs;
using Guardian.Application.Parameters;
using Guardian.Application.Wrappers;
using Guardian.Domain.User.Enums;
using Guardian.Domain.User.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Guardian.WebApi.Controllers.v1
{
    [Route("usuarios/")]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;

        public AccountController(IAccountService accountService, ITokenService tokenService)
        {
            _accountService = accountService;
            _tokenService = tokenService;
        }

        [HttpGet("me/")]
        [Authorize]
        public async Task<IActionResult> GetAuthenticatedUserAsync()
        {
            int authenticatedUserId = AuthenticatedUser.GetId();

            var result = await _accountService.GetAccountByIdAsync(authenticatedUserId);

            if (!result.Succeeded)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = nameof(ERoles.SuperAdmin))]
        public async Task<IActionResult> GetPagedAsync([FromQuery] RequestParameter parameter)
        {
            var result = await _accountService.GetPagedAsync(parameter);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("buscar")]
        [Authorize(Roles = nameof(ERoles.SuperAdmin))]
        public async Task<IActionResult> SearchByUserAsync([FromQuery] UserFilter filter, [FromQuery] RequestParameter parameter)
        {
            return Ok(await _accountService.SearchByUserAsync(filter, parameter));
        }

        [HttpPost("auth/")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticationRequest request)
        {
            var result = await _accountService.AuthenticateAsync(request);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            _tokenService.SetTokensToCookies(
                result.Data.AccessToken,
                result.Data.RefreshToken,
                HttpContext
            );

            return Ok(result);
        }

        [HttpPost("cadastro/")]
        [Authorize(Roles = nameof(ERoles.SuperAdmin))]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
        {
            var result = await _accountService.RegisterAsync(request);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{userId}")]
        [Authorize(Roles = nameof(ERoles.SuperAdmin))]
        public async Task<IActionResult> UpdateAsync(int userId, [FromBody] UpdateAccountRequest request)
        {
            var result = await _accountService.UpdateAsync(userId, request);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("{userId}/cargos")]
        [Authorize(Roles = nameof(ERoles.SuperAdmin))]
        public async Task<IActionResult> UpdateRolesAsync(int userId, [FromBody] UpdateRolesRequest request)
        {
            var result = await _accountService.UpdateRolesAsync(userId, request);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("{userId}/status")]
        [Authorize(Roles = nameof(ERoles.SuperAdmin))]
        public async Task<IActionResult> UpdateIsActiveAsync(int userId)
        {
            var result = await _accountService.UpdateIsActiveAsync(userId);

            if (!result.Succeeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("refresh/")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            HttpContext.Request.Cookies.TryGetValue("access_token", out string accessToken);
            HttpContext.Request.Cookies.TryGetValue("refresh_token", out var refreshToken);

            // Se um dos tokens for nulo, a validação não ocorre, pois automaticamente saberemos que o usuário não é válido  
            if (accessToken is null || refreshToken is null)
            {
                return Unauthorized(
                    Response<string>.Failure(["Acesso negado. Autentique novamente."])
                );
            }

            RefreshTokenRequest request = new(accessToken, refreshToken);

            var result = await _accountService.RefreshTokenAsync(request);

            if (!result.Succeeded)
            {
                return Unauthorized(result);
            }

            _tokenService.SetTokensToCookies(
                result.Data.AccessToken,
                result.Data.RefreshToken,
                HttpContext
            );

            return Ok(result);
        }

        [HttpPost("revogar/")]
        public IActionResult RevokeTokens()
        {
            HttpContext.Response.Cookies.Delete("access_token");
            HttpContext.Response.Cookies.Delete("refresh_token");

            Response<string> response = Response<string>.Success(
                message: "Sucesso.",
                data: string.Empty
            );

            return Ok(response);
        }

        [HttpPost("esqueci-minha-senha/")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordRequest request)
        {
            var result = await _accountService.ForgotPasswordAsync(request.Email, Request.Headers.Origin);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("recuperar-senha/")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest request)
        {
            var result = await _accountService.ResetPasswordAsync(request);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("alterar-senha/")]
        [Authorize]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest request)
        {
            int userId = AuthenticatedUser.GetId();

            var result = await _accountService.ChangePasswordAsync(userId, request);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

    }
}