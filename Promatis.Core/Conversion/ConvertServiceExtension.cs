using System.Collections.Generic;
using System.Linq;

namespace Promatis.Core.Conversion
{
    /// <summary>
    /// Методы конвертации, расширяющие функциональность типа <see cref="object"/> 
    /// </summary>
    public static class ConvertExtensions
    {
        private static IConversionService _autoConverterService;
        private static IConversionService AutoConverterService => _autoConverterService ?? (_autoConverterService = IoC.Resolve<IConversionManager>().ConversionService);

        /// <summary>
        /// Сбрасывает все настройки конвертации
        /// </summary>
        public static void Reset() => _autoConverterService = null;

        /// <summary>
        /// Выполняет конвертацию в целевой объект
        /// </summary>
        /// <typeparam name="TDest">Тип целевого объекта</typeparam>
        /// <param name="source">Исходный объект</param>
        /// <param name="destination">Целевой объект</param>
        /// <returns>Целевой объект</returns>
        public static TDest ConvertTo<TDest>(this object source, TDest destination) => AutoConverterService.MapTo(source, destination);

        /// <summary>
        /// Создает объект целевого типа и выполняет конвертацию в него
        /// </summary>
        /// <typeparam name="TDest">Тип целевого объекта</typeparam>
        /// <param name="source">Исходный объект</param>
        /// <returns>Целевой объект</returns>
        public static TDest ConvertTo<TDest>(this object source) => AutoConverterService.MapTo<TDest>(source);

        /// <summary>
        /// Создает объект целевого типа и выполняет конвертацию в него
        /// </summary>
        /// <typeparam name="TSrc">Тип исходного объекта</typeparam>
        /// <typeparam name="TDest">Тип целевого объекта</typeparam>
        /// <param name="source">Текущий объект</param>
        /// <returns>Целевой объект</returns>
        public static TDest ConvertTo<TSrc, TDest>(this TSrc source) => AutoConverterService.MapTo<TSrc, TDest>(source);

        /// <summary>
        /// Создает проекцию для LINQ запроса
        /// </summary>
        /// <typeparam name="TSrc">Тип исходного объекта</typeparam>
        /// <typeparam name="TDest">Тип целевого объекта</typeparam>
        /// <param name="query">Linq запрос</param>
        /// <returns>Linq запрос с проекцией</returns>
        public static IQueryable<TDest> ProjectToQueryable<TSrc, TDest>(this IQueryable<TSrc> query) => AutoConverterService.ProjectToQuery<TSrc, TDest>(query);

        /// <summary>
        /// Создает проекцию для LINQ запроса, и возвращает коллекцию целевых объектов
        /// </summary>
        /// <typeparam name="TSrc">Тип исходного объекта</typeparam>
        /// <typeparam name="TDest">Тип целевого объекта</typeparam>
        /// <param name="query">Linq запрос</param>
        /// <returns>Список в памяти в проекции</returns>
        public static List<TDest> ProjectToList<TSrc, TDest>(this IQueryable<TSrc> query) => AutoConverterService.ProjectToList<TSrc, TDest>(query);
    }
}
