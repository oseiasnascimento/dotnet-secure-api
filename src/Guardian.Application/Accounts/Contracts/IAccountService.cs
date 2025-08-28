using Guardian.Application.Accounts.Dtos.Inputs;
using Guardian.Application.Accounts.Dtos.Outputs;
using Guardian.Application.Parameters;
using Guardian.Application.Wrappers;
using Guardian.Domain.User.Filters;

namespace Guardian.Application.Accounts.Contracts
{
    public interface IAccountService
    {
        /// <summary>
        /// Recupera uma conta de acordo com o Id passado.
        /// </summary>
        /// <param name="id">Id do usuário</param>
        /// <returns>Retorna uma Response com os dados do usuário</returns>
        public Task<Response<GetAccountDto>> GetAccountByIdAsync(int id);

        /// <summary>
        /// Recupera todos os usuarios cadastrados de forma paginada.
        /// </summary>
        /// <param name="parameter">Parametros de paginação.</param>
        /// <returns>Retorna uma response paginada com todas as contas recuperadas.</returns>
        public Task<PagedResponse<IEnumerable<GetAccountDto>>> GetPagedAsync(RequestParameter parameter);

        /// <summary>
        /// Faz a busca de usuários de acordo com os parâmetros passados.
        /// </summary>
        /// <param name="filter">Filtro de busca</param>
        /// <param name="parameter">Parâmetros de paginação</param>
        /// <returns>Retorna uma lista paginada de usuários</returns>
        public Task<PagedResponse<IEnumerable<GetAccountDto>>> SearchByUserAsync(UserFilter filter, RequestParameter parameter);

        /// <summary>
        /// Realiza a autenticação do usuário.
        /// </summary>
        /// <param name="request">Requisição necessária para autenticação.</param>
        /// <returns>Retorna uma Response com os dados do usuário autenticado.</returns>
        public Task<Response<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request);

        /// <summary>
        /// Realiza a autenticação de um passageiro.
        /// </summary>
        /// <param name="request">Requisição necessária para autenticação.</param>
        /// <returns>Retorna uma Response com os dados do usuário autenticado.</returns>
        public Task<Response<AuthenticationResponse>> AuthenticateUserAsync(AuthenticationRequest request);

        /// <summary>
        /// Realiza o cadastro de usuário.
        /// </summary>
        /// <param name="request">Request para cadastro, contendo propriedades personalizáveis.</param>
        /// <returns>Retorna uma Response de sucesso ou de erro de acordo com os parâmetros passados.</returns>
        public Task<Response<string>> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// Método para atualizar um usuário no sistema.
        /// </summary>
        /// <param name="userId">Id do usuário a ser atualizado</param>
        /// <param name="request">Requisição necessária</param>
        /// <returns>Retorna uma response com a conta do usuário atualizada.</returns>
        public Task<Response<GetAccountDto>> UpdateAsync(int userId, UpdateAccountRequest request);

        /// <summary>
        /// Atualiza os cargos de um usuário específico.
        /// Precisa conter todos os cargos necessários para o usuário
        /// </summary>
        /// <param name="userId">Id do usuário</param>
        /// <param name="roles">Novos cargos.</param>
        /// <returns>Retorna uma response de sucesso ou falha.</returns>
        public Task<Response<string>> UpdateRolesAsync(int userId, UpdateRolesRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<Response<string>> UpdateIsActiveAsync(int userId);

        /// <summary>
        /// Gera um novo Refresh Token, junto com a data de validade definida nas settings.
        /// </summary>
        /// <param name="request">A Request contém o Access Token (que será revogado) e o novo Refresh Token criado a partir do Access Token.</param>
        /// <returns>Retorna uma Response de sucesso ou de erro de acordo com os parâmetros passados.</returns>
        public Task<Response<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);

        /// <summary>
        /// Faz a revogação dos tokens (Access Token e Refresh Token).
        /// </summary>
        /// <param name="userId">Id do usuário que está tendo os tokens revogados.</param>
        /// <returns>Retorna uma Response de sucesso ou de erro de acordo com os parâmetros passados.</returns>
        public Task<Response<string>> RevokeRefreshTokenAsync(int userId);

        /// <summary>
        /// Envia o email para recuperação de senha de acordo com o email passado.
        /// </summary>
        /// <param name="email">Email do usuário.</param>
        /// <param name="origin">Origem da requisição, necessária para montar a url retornada.</param>
        /// <returns>Uma response de sucesso ou de erro.</returns>
        public Task<Response<string>> ForgotPasswordAsync(string email, string origin);

        /// <summary>
        /// Efetua a alteração de senha do usuário. As senhas precisam coincidir, 
        /// o e-mail ser correto e o token ser válido.
        /// </summary>
        /// <param name="request">Request com todos os campos necessários para alteração de senha.</param>
        /// <returns>Uma response de sucesso ou de erro.</returns>
        public Task<Response<string>> ResetPasswordAsync(ResetPasswordRequest request);

        /// <summary>
        /// Efetua a alteração de senha do usuário. As senhas precisam coincidir.
        /// </summary>
        /// <param name="userId">Id do usuário.</param>
        /// <param name="request">Request com todos os campos necessários para alteração de senha.</param>
        /// <returns>Uma response de sucesso ou de erro.</returns>
        public Task<Response<string>> ChangePasswordAsync(int userId, ChangePasswordRequest request);
    }
}
