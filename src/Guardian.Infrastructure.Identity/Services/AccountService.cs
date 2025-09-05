using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Guardian.Domain.User.Entities;
using Guardian.Application.Accounts.Contracts;
using Guardian.Application.Shared.Contracts;
using Guardian.Application.Wrappers;
using Guardian.Application.Parameters;
using Guardian.Application.Accounts.Dtos.Inputs;
using Guardian.Application.Accounts.Dtos.Outputs;
using Guardian.Application.Shared.Dtos;
using Guardian.Domain.User.Enums;
using Guardian.Domain.User.Filters;
using Guardian.Domain.User.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Identity.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IRoleRepository _roleRepository;

        public AccountService(UserManager<ApplicationUser> userManager,
                              ITokenService tokenService,
                              IEmailService emailService,
                              IRoleRepository roleRepository,
                              IUserRepository userRepository)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
        }

        public async Task<Response<GetAccountDto>> GetAccountByIdAsync(int id)
        {
            ApplicationUser user = await _userManager.Users.AsNoTracking()
                                                           .Where(x => x.Id == id)
                                                           .FirstOrDefaultAsync()
                                                           .ConfigureAwait(false);
            if (user is null)
            {
                return Response<GetAccountDto>.Failure(
                    ["Usuário não encontrado."]
                );
            }

            IEnumerable<string> roles = await _userManager.GetRolesAsync(user);

            GetAccountDto mappedAccount = GetAccountDto.Map(user, roles);

            return Response<GetAccountDto>.Success(
                message: "Usuário recuperado com sucesso.",
                data: mappedAccount
            );
        }

        public async Task<PagedResponse<IEnumerable<GetAccountDto>>> GetPagedAsync(RequestParameter parameter)
        {
            // Filtro para recuperar apenas usuários que são Admin
            IEnumerable<ApplicationUser> users = await _userManager.Users.AsNoTracking()
                                                                        .Include(x => x.UserRoles)
                                                                        .ThenInclude(x => x.Role)
                                                                        .Where(x => x.UserRoles.Any(ur => ur.Role.Name != nameof(ERoles.User)) &&
                                                                                (x.UserRoles.Count > 1 || x.UserRoles.All(ur => ur.Role.Name != nameof(ERoles.User))))
                                                                        .Skip((parameter.PageNumber - 1) * parameter.PageSize)
                                                                        .Take(parameter.PageSize)
                                                                        .ToListAsync()
                                                                        .ConfigureAwait(false);

            int totalUsers = await _userManager.Users.AsNoTracking()
                                                     .Where(x => x.UserRoles.Any(ur => ur.Role.Name != nameof(ERoles.User)) &&
                                                            (x.UserRoles.Count > 1 ||
                                                             x.UserRoles.All(ur => ur.Role.Name != nameof(ERoles.User))))
                                                     .CountAsync()
                                                     .ConfigureAwait(false);

            IEnumerable<GetAccountDto> mappedUsers = GetAccountDto.Map(users);

            return new PagedResponse<IEnumerable<GetAccountDto>>(
                data: mappedUsers,
                total: totalUsers,
                pageNumber: parameter.PageNumber,
                pageSize: parameter.PageSize
            );
        }

        public async Task<PagedResponse<IEnumerable<GetAccountDto>>> SearchByUserAsync(UserFilter filter, RequestParameter parameter)
        {
            var users = await _userRepository.SearchByUserAsync(filter, parameter.PageNumber, parameter.PageSize);
            IEnumerable<GetAccountDto> mappedUsers = GetAccountDto.Map(users.users);

            return new PagedResponse<IEnumerable<GetAccountDto>>(
                data: mappedUsers,
                total: users.total,
                pageNumber: parameter.PageNumber,
                pageSize: parameter.PageSize
            );
        }

        public async Task<Response<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(request.CPF);
            if (user is null)
            {
                return Response<AuthenticationResponse>.Failure(
                    errors: ["CPF ou senha inválidos."]
                );
            }

            if (!user.IsActive)
            {
                return Response<AuthenticationResponse>.Failure(
                    errors: ["CPF ou senha inválidos."]
                );
            }

            bool passwordIsValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (passwordIsValid == false)
            {
                return Response<AuthenticationResponse>.Failure(
                    errors: ["CPF ou senha inválidos."]
                );
            }

            IEnumerable<string> roles = await _userManager.GetRolesAsync(user);
            List<string> availableRoles = [
                nameof(ERoles.SuperAdmin),
                nameof(ERoles.Admin),
                nameof(ERoles.User),
                nameof(ERoles.ReadOnly)
            ];
            bool roleIsValid = roles.Any(availableRoles.Contains);

            if (!roleIsValid)
            {
                return Response<AuthenticationResponse>.Failure(
                    errors: ["CPF ou senha inválidos."]
                );
            }

            List<Claim> claims = GenerateClaims(user, roles, false);
            RefreshTokenResponse tokens = await GenerateTokensAsync(user, claims);

            AuthenticationResponse response = AuthenticationResponse.Map(user, roles, tokens.AccessToken, tokens.RefreshToken);

            user.LastLogin = DateTime.Now;

            await _userManager.UpdateAsync(user);

            return Response<AuthenticationResponse>.Success(
                message: "Autenticação realizada com sucesso",
                data: response
            );
        }

        public async Task<Response<AuthenticationResponse>> AuthenticateUserAsync(AuthenticationRequest request)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(request.CPF);
            if (user is null)
            {
                return Response<AuthenticationResponse>.Failure(
                    errors: ["Usuário ou senha inválidos."]
                );
            }

            if (!user.IsActive)
            {
                return Response<AuthenticationResponse>.Failure(
                    errors: ["CPF ou senha inválidos."]
                );
            }

            bool passwordIsValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (passwordIsValid == false)
            {
                return Response<AuthenticationResponse>.Failure(
                    errors: ["Usuário ou senha inválidos."]
                );
            }

            IEnumerable<string> roles = await _userManager.GetRolesAsync(user);
            List<Claim> claims = GenerateClaims(user, roles, true);
            RefreshTokenResponse tokens = await GenerateTokensAsync(user, claims);

            AuthenticationResponse response = AuthenticationResponse.Map(user, roles, tokens.AccessToken, tokens.RefreshToken);

            user.LastLogin = DateTime.Now;

            await _userManager.UpdateAsync(user);

            return Response<AuthenticationResponse>.Success(
                data: response,
                message: "Autenticação realizada com sucesso."
            );
        }

        public async Task<Response<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
        {
            ClaimsPrincipal claimsPrincipal = _tokenService.GetPrincipalForExpiredToken(request.AccessToken);
            if (claimsPrincipal is null)
            {
                return Response<RefreshTokenResponse>.Failure(
                    errors: ["Access/Refresh token inválidos ou expirados."]
                );
            }

            string userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value;

            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user is null ||
                user.RefreshToken != request.RefreshToken ||
                user.RefreshTokenValidity <= DateTime.Now)
            {
                return Response<RefreshTokenResponse>.Failure(
                    errors: ["Access/Refresh token inválidos ou expirados."]
                );
            }

            List<Claim> claims = [.. claimsPrincipal.Claims];

            RefreshTokenResponse tokensResponse = await GenerateTokensAsync(user, claims);

            user.LastLogin = DateTime.UtcNow;
            user.RefreshToken = tokensResponse.RefreshToken;
            await _userManager.UpdateAsync(user);

            return Response<RefreshTokenResponse>.Success(
                message: "Successo.",
                data: tokensResponse
            );
        }

        public async Task<Response<string>> RegisterAsync(RegisterRequest request)
        {
            ApplicationUser user = new()
            {
                CPF = request.CPF,
                FullName = request.FullName,
                UserName = request.CPF,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                RefreshToken = "",
                RefreshTokenValidity = DateTime.MinValue
            };

            // Validando se todos os cargos passados na requisição são válidos
            List<string> existentRoles = await _roleRepository.GetRolesAsync();
            List<string> invalidRoles = request.Roles.Except(existentRoles).ToList();

            if (invalidRoles.Count != 0)
            {
                return Response<string>.Failure(
                    errors: [$"Cargos inválidos encontrados: {string.Join(", ", invalidRoles)}."]
                );
            }

            IdentityResult result = await _userManager.CreateAsync(user, request.Password);
            Dictionary<string, string> identityErrorMapping = new()
            {
                { "DuplicateUserName", "Esse CPF já está em uso." },
                { "DuplicateEmail", "Esse e-mail já está em uso." }
            };

            if (result.Errors.Any())
            {
                if (identityErrorMapping.TryGetValue(result.Errors.FirstOrDefault().Code, out string errorDescription))
                {
                    return Response<string>.Failure(
                        errors: [errorDescription]
                    );
                }
            }

            IdentityResult addToRolesResult = await _userManager.AddToRolesAsync(user, request.Roles);
            if (addToRolesResult.Errors.Any())
            {
                return Response<string>.Failure(
                    ["Ocorreu um erro ao tentar adicionar o usuário ao cargo indicado. Verifique com o time de suporte e tente novamente."]
                );
            }

            // SendMailRequest sendMailRequest = new()
            // {
            //     To = user.Email,
            //     Subject = "Bem-Vindo(a) - Navega MA",
            //     TemplatePath = "WelcomeTemplate.html",
            //     Parameters = {
            //         { "user.FullName", user.FullName }
            //     }
            // };

            // Removendo envio de e-mail devido à falta da disponibilização de um e-mail "noreply"
            // await _emailService.SendEmailAsync(sendMailRequest);

            return Response<string>.Success(
                data: string.Empty,
                message: "Usuário cadastrado com sucesso."
            );
        }

        public async Task<Response<GetAccountDto>> UpdateAsync(int userId, UpdateAccountRequest request)
        {
            ApplicationUser user = await _userRepository.GetUserByIdAsync(userId);
            if (user is null)
            {
                return Response<GetAccountDto>.Failure(
                    errors: ["Usuário não encontrado."]
                );
            }

            user = UpdateAccountRequest.Map(user, request);

            IdentityResult result = await _userManager.UpdateAsync(user);
            Dictionary<string, string> identityErrorMapping = new()
            {
                { "DuplicateUserName", "Esse CPF já está em uso." },
                { "DuplicateEmail", "Esse e-mail já está em uso." }
            };

            if (result.Errors.Any())
            {
                if (identityErrorMapping.TryGetValue(result.Errors.FirstOrDefault().Code, out string errorDescription))
                {
                    return Response<GetAccountDto>.Failure(
                        errors: [errorDescription]
                    );
                }
            }

            Response<string> updateRoles = await UpdateRolesAsync(userId, new() { Roles = request.Roles });
            if (!updateRoles.Succeeded)
            {
                return Response<GetAccountDto>.Failure(
                    updateRoles.Errors
                );
            }

            IEnumerable<string> roles = await _userManager.GetRolesAsync(user);
            GetAccountDto mappedUser = GetAccountDto.Map(user, roles);

            return Response<GetAccountDto>.Success(
                message: "Usuário atualizado com sucesso.",
                data: mappedUser
            );
        }

        public async Task<Response<string>> UpdateRolesAsync(int userId, UpdateRolesRequest request)
        {
            ApplicationUser user = await _userRepository.GetUserByIdAsync(userId);
            if (user is null)
            {
                return Response<string>.Failure(
                    errors: ["Usuário não encontrado."]
                );
            }

            List<string> existentRoles = await _roleRepository.GetRolesAsync();
            List<string> invalidRoles = [.. request.Roles.Except(existentRoles)];
            if (invalidRoles.Count != 0)
            {
                return Response<string>.Failure(
                    errors: [$"Cargos inválidos encontrados: {string.Join(", ", invalidRoles)}."]
                );
            }

            IEnumerable<string> currentRoles = await _userManager.GetRolesAsync(user);
            List<string> rolesToAdd = [.. request.Roles.Except(currentRoles)];
            List<string> rolesToRemove = [.. currentRoles.Except(request.Roles)];
            if (rolesToAdd.Count != 0)
            {
                await _userManager.AddToRolesAsync(user, rolesToAdd);
            }

            if (rolesToRemove.Count != 0)
            {
                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            }

            return Response<string>.Success(
                message: "Cargos atualizados com sucesso.",
                data: string.Empty
            );
        }

        public async Task<Response<string>> UpdateIsActiveAsync(int userId)
        {
            ApplicationUser user = await _userRepository.GetByIdAsync(userId);
            if (user is null)
            {
                return Response<string>.Failure(
                    errors: ["Usuário não encontrado."]
                );
            }

            // Simplificando a atualização de status de usuário
            // Se estiver ativo, se torna inativo
            // Se estiver inativo, se torna ativo
            user.IsActive = !user.IsActive;

            await _userManager.UpdateAsync(user);

            return Response<string>.Success(
                message: "Status atualizado com sucesso.",
                data: string.Empty
            );
        }

        private async Task<RefreshTokenResponse> GenerateTokensAsync(ApplicationUser user, List<Claim> claims)
        {
            // Gerando Tokens
            JwtSecurityToken accessToken = _tokenService.GenerateAccessToken(claims);
            string refreshToken = _tokenService.GenerateRefreshToken();
            string accessTokenAsString = new JwtSecurityTokenHandler().WriteToken(accessToken);

            // Salvando dados do RefreshToken no Banco de Dados
            user.RefreshToken = refreshToken;
            user.RefreshTokenValidity = DateTime.Now.AddDays(7);
            // user.LastLogin = DateTime.Now;
            await _userManager.UpdateAsync(user);

            return new RefreshTokenResponse()
            {
                AccessToken = accessTokenAsString,
                RefreshToken = refreshToken
            };
        }

        private static List<Claim> GenerateClaims(ApplicationUser user, IEnumerable<string> roles, bool isPassenger)
        {
            List<Claim> claims =
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("isUserAuth", isPassenger.ToString().ToLower()),
                new Claim("userId", user.Id.ToString())
            ];

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        public async Task<Response<string>> RevokeRefreshTokenAsync(int userId)
        {
            ApplicationUser user = await _userManager.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId)
                .ConfigureAwait(false);

            if (user is null)
            {
                return Response<string>.Failure(
                    errors: ["Usuário autenticado não encontrado."]
                );
            }

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);

            return Response<string>.Success(
                data: string.Empty,
                message: "Tokens revogados com sucesso."
            );
        }

        public async Task<Response<string>> ForgotPasswordAsync(string email, string origin)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return Response<string>.Failure(
                    errors: ["Usuário não encontrado. Verifique e tente novamente."]
                );
            }

            // Gerando token para recuperação de senha (após gerar o token com o Identity, ele é criptografado para aumentar a segurança)
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            byte[] encodingToken = Encoding.UTF8.GetBytes(token);
            string validToken = WebEncoders.Base64UrlEncode(encodingToken);

            string url = $"{origin}/auth/nova-senha?email={user.Email}&token={validToken}";

            SendMailRequest sendMailRequest = new()
            {
                To = user.Email,
                Subject = "Recuperar Senha - Guardian",
                TemplatePath = "ForgotPasswordTemplate.html",
                Parameters = {
                    { "user.FullName", user.FullName },
                    { "{{url}}", url }
                }
            };

            await _emailService.SendEmailAsync(sendMailRequest);

            return Response<string>.Success(
                message: "Link de recuperação de senha enviado para o seu e-mail.",
                data: string.Empty
            );
        }

        public async Task<Response<string>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return Response<string>.Failure(
                    errors: ["Usuário não encontrado. Verifique e tente novamente."]
                );
            }

            bool newPasswordEqualsOldPassword = await _userManager.CheckPasswordAsync(user, request.Password);
            if (newPasswordEqualsOldPassword)
            {
                return Response<string>.Failure(
                    errors: ["Sua nova senha precisa ser diferente da senha anterior."]
                );
            }

            // Descriptografando o token para conseguir usá-lo na recuperação de senha
            byte[] decodedToken = WebEncoders.Base64UrlDecode(request.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            IdentityResult resetPasswordResult = await _userManager.ResetPasswordAsync(user, normalToken, request.Password);
            if (resetPasswordResult.Succeeded == false)
            {
                return Response<string>.Failure(
                    errors: ["Ocorreu um problema na recuperação da sua senha."]
                );
            }

            return Response<string>.Success(
                message: "Senha alterada com sucesso.",
                data: string.Empty
            );
        }

        public async Task<Response<string>> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            // Oseias 10/12/2024
            // Verificar se a nova senha e a confirmação são iguais
            if (request.NewPassword != request.ConfirmNewPassword)
            {
                return Response<string>.Failure(
                    errors: ["A nova senha e a confirmação não coincidem."]
                );
            }

            ApplicationUser user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                return Response<string>.Failure(
                    errors: ["Usuário não encontrado. Verifique e tente novamente."]
                );
            }

            bool newPasswordEqualsOldPassword = await _userManager.CheckPasswordAsync(user, request.NewPassword);
            if (newPasswordEqualsOldPassword)
            {
                return Response<string>.Failure(
                    errors: ["Sua nova senha precisa ser diferente da senha anterior."]
                );
            }

            // Oseias 10/12/2024
            // Tentar alterar a senha
            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                return Response<string>.Failure(
                    errors: ["Ocorreu um problema ao alterar sua senha."]
                );
            }

            // Oseias 10/12/2024
            // Atualizar a propriedade indicando que o usuário não está mais usando a senha padrão
            user.IsDefaultPassword = false;
            await _userManager.UpdateAsync(user);

            return Response<string>.Success(
                message: "Senha alterada com sucesso.",
                data: string.Empty
            );
        }
    }
}