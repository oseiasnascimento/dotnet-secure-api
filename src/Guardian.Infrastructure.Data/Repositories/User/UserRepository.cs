using Microsoft.EntityFrameworkCore;
using Guardian.Domain.User.Entities;
using Guardian.Domain.User.Repositories;
using Guardian.Domain.User.Filters;
using Guardian.Infrastructure.Data.Contexts;
using Guardian.Infrastructure.Data.Repositories.Common;

namespace Guardian.Infrastructure.Data.Repositories
{
    public class UserRepository : GenericRepository<ApplicationUser>, IUserRepository
    {
        private readonly DbSet<ApplicationUser> _users;

        public UserRepository(ApplicationDbContext context) : base(context)
        {
            _users = context.Users;
        }

        public async Task<ApplicationUser> GetUserByIdAsync(int id)
        {
            return await _users.Include(x => x.UserRoles)
                               .ThenInclude(x => x.Role)
                               .Where(x => x.Id == id)
                               .FirstOrDefaultAsync()
                               .ConfigureAwait(false);
        }

        public async Task<(IEnumerable<ApplicationUser> users, int total)> SearchByUserAsync(UserFilter filter, int pageNumber, int pageSize)
        {
            IEnumerable<ApplicationUser> users = await _users.AsNoTracking()
                                                             .Include(x => x.UserRoles)
                                                             .ThenInclude(x => x.Role)
                                                             .Where(x => x.FullName.Contains(filter.Name) || x.CPF.Contains(filter.CPF))
                                                             .Skip((pageNumber - 1) * pageSize)
                                                             .Take(pageSize)
                                                             .ToListAsync()
                                                             .ConfigureAwait(false);

            int totalUsers = await _users.AsNoTracking()
                                         .Where(x => x.FullName.Contains(filter.Name) ||x.CPF
                                         .Contains(filter.CPF))
                                         .CountAsync()
                                         .ConfigureAwait(false);

            return (users, totalUsers);
        }
    }
}
