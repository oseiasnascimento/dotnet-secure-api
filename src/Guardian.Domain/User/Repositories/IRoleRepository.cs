using Guardian.Domain.Common.Repositories;
using Guardian.Domain.User.Entities;

namespace Guardian.Domain.User.Repositories
{
    public interface IRoleRepository : IGenericRepository<ApplicationRole>
    {
        /// <summary>
        /// Recupera o nome de todas as roles.
        /// </summary>
        /// <returns>Retorna uma lista com os nomes dos cargos.</returns>
        public Task<List<string>> GetRolesAsync();
    }
}