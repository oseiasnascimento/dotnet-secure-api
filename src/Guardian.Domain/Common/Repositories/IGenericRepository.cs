using Microsoft.EntityFrameworkCore;

namespace Guardian.Domain.Common.Repositories
{
    /// <summary>
    /// Repositório genérico que possui tipo <typeparamref name="T"/>, que deve ser uma classe;
    /// </summary>
    /// <typeparam name="T">class</typeparam>
    public interface IGenericRepository<T> where T : class
    {
        public DbContext GetDbContext();

        public Task<IReadOnlyList<T>> GetPagedAsync(int pageNumber, int pageSize);
        public Task<ICollection<T>> GetAllAsync();
        public Task<T> GetByIdAsync(int? id);
        public IQueryable<T> Select();
        public Task<int> CountAsync();
        public Task<T> CreateAsync(T entity);
        public Task<T> AddAsync(T entity);
        public void UpdateWithoutSave(T entity);
        public Task UpdateAsync(T entity);
        public Task DeleteAsync(T entity);
        public Task SaveAsync();

        public Task BeginTransactionAsync();
        public Task CommitAsync();
        public Task RollbackAsync();
    }
}