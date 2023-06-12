namespace Application.Extensions
{
    using System.Linq.Expressions;

    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    using Shared;

    public static class QueryableExtensions
    {
        public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T>(this IMongoQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken) where T : class
        {
            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            pageSize = pageSize == 0 ? 10 : pageSize;
            int count = await source.CountAsync(cancellationToken);
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            List<T> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return PaginatedResult<T>.Create(items, count, pageNumber, pageSize);
        }

        public static PaginatedResult<T> ToPaginatedList<T>(this IQueryable<T> source, int pageNumber, int pageSize) where T : class
        {
            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            pageSize = pageSize == 0 ? 10 : pageSize;
            int count =  source.Count();
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            List<T> items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return PaginatedResult<T>.Create(items, count, pageNumber, pageSize);
        }

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
