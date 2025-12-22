using System.Linq;

namespace Promatis.Core.Extensions
{
    /// <summary>
    /// Методы расширения для типа <see cref="IQueryable"/>
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Возвращает запрос для получения данных на указаной странице с заданной размерностью
        /// </summary>
        /// <typeparam name="TEntity">Тип данных</typeparam>
        /// <param name="query">Запрос</param>
        /// <param name="page">Номер страницы</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <returns>Запрос</returns>
        public static IQueryable<TEntity> Page<TEntity>(this IQueryable<TEntity> query, int page, int pageSize)
        {
            return query.Skip(pageSize * (page - 1)).Take(pageSize);
        }
    }
}
