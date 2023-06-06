namespace Persistence.Repository
{
    using System.Linq.Expressions;

    using MongoDB.Driver;

    using Domain.Common.Interfaces;

    public interface IMongoRepository<T> where T : class, IEntity
    {
        Task<T> GetByIdAsync(string id);

        Task<T> GetAsync(Expression<Func<T, bool>> predicate);

        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);

        ReplaceOneResult Update(T entity);
        Task UpdateAsync(T entity);
        Task InsertAsync(T entity);

        Task DeleteAsync(string id);

        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        Task<ICollection<T>> FindDistinctAsync(string field, FilterDefinition<T> filter);

        List<T> GetAll();
    }
}
