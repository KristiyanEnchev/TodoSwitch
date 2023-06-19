namespace Application.Extensions
{
    using System.Linq.Expressions;

    using Shared;

    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    public static class QueryableExtensions
    {
        public static async Task<PaginatedResult<TResult>> ToPaginatedListAsync<TSource, TResult>(
            this IQueryable<TSource> source,
            int pageNumber,
            int pageSize,
            Func<TSource, TResult> mapper,
            CancellationToken cancellationToken)
            where TSource : class
            where TResult : class
        {
            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            pageSize = pageSize == 0 ? 10 : pageSize;
            int count = source.Count();
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            List<TSource> items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            List<TResult> mappedItems = items.Select(mapper).ToList();
            return PaginatedResult<TResult>.Create(mappedItems, count, pageNumber, pageSize);
        }

        public static IMongoQueryable<T> Sort<T, TKey>(this IMongoQueryable<T> queryable, Expression<Func<T, TKey>> keySelector, bool ascending)
        {
            return ascending ? queryable.OrderBy(keySelector) : queryable.OrderByDescending(keySelector);
        }
    }
}