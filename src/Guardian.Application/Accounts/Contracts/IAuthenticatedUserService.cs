namespace Guardian.Application.Accounts.Contracts
{
    public interface IAuthenticatedUserService
    {
        /// <summary>
        /// Id do usuário autenticado.
        /// </summary>
        public int Id { get; }

        public int GetId();

    }
}
