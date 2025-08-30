using Guardian.Domain.Common.Repositories;
using Guardian.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Guardian.Infrastructure.Data.Repositories.Common
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public DbContext GetDbContext()
        {
            return _context;
        }

        public async Task<ICollection<T>> GetAllAsync()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        public IQueryable<T> Select()
        {
            return _context.Set<T>().AsQueryable();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Set<T>().AsNoTracking().CountAsync();
        }

        public async Task<IReadOnlyList<T>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _context
                        .Set<T>()
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .AsNoTracking()
                        .ToListAsync()
                        .ConfigureAwait(false);
        }

        public async Task<T> GetByIdAsync(int? id)
        {
            return await _context.Set<T>().FindAsync(id).ConfigureAwait(false);
        }

        public async Task<T> CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await SaveAsync();
            return entity;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public void UpdateWithoutSave(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await SaveAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await SaveAsync();
        }

        public Task SaveAsync()
        {
            return _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }
    }
}