using Microsoft.EntityFrameworkCore;
using Guardian.Domain.User.Entities;
using Guardian.Domain.User.Repositories;
using Guardian.Infrastructure.Data.Contexts;
using Guardian.Infrastructure.Data.Repositories.Common;

namespace Guardian.Infrastructure.Data.Repositories
{
    public class RoleRepository : GenericRepository<ApplicationRole>, IRoleRepository
    {
        private readonly DbSet<ApplicationRole> _roles;

        public RoleRepository(ApplicationDbContext context) : base(context)
        {
            _roles = context.Set<ApplicationRole>();
        }

        public async Task<List<string>> GetRolesAsync()
        {
            return await _roles.AsNoTracking()
                            .Select(x => x.Name)
                            .ToListAsync()
                            .ConfigureAwait(false);
        }
    }
}