using Guardian.Domain.Common.Repositories;
using Guardian.Domain.User.Entities;
using Guardian.Domain.User.Filters;

namespace Guardian.Domain.User.Repositories
{
    public interface IUserRepository : IGenericRepository<ApplicationUser>
    {
        /// <summary>
        /// Busca por uma lista de usuários de acordo com o filtro passado.
        /// </summary>
        /// <param name="filter">Filtros para busca de usuário</param>
        /// <returns>Uma lista de usuários e a contagem total da busca</returns>
        public Task<(IEnumerable<ApplicationUser> users, int total)> SearchByUserAsync(UserFilter filter, int pageNumber, int pageSize);

        /// <summary>
        /// Recupera um usuário por id
        /// </summary>
        /// <param name="id">Id do usuário</param>
        /// <returns>Retorna um usuário com cargos</returns>
        public Task<ApplicationUser> GetUserByIdAsync(int id);
    }
}