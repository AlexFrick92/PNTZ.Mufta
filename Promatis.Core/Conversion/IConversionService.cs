using System.Collections.Generic;
using System.Linq;

namespace Promatis.Core.Conversion
{
    /// <summary>
    /// Интерфейс сервиса конвертации объекта одного типа в объект другого типа. 
    /// <para>Используется внутри методов расширения для объектов</para>
    /// </summary>
    public interface IConversionService
    {
        /// <summary>
        /// Маппинг объект - объект
        /// </summary>
        /// <typeparam name="TDest">Тип целевого объекта</typeparam>
        /// <param name="source">Объект - источник</param>
        /// <param name="destination">Целевой объект</param>
        /// <returns>Целевой объект</returns>
        TDest MapTo<TDest>(object source, TDest destination);

        /// <summary>
        /// Маппинг объект - объект
        /// </summary>
        /// <typeparam name="TDest">Тип целевого объекта</typeparam>
        /// <param name="source">Объект - источник</param>
        /// <returns>Целевой объект</returns>
        TDest MapTo<TDest>(object source);

        /// <summary>
        /// Маппинг объект - объекта
        /// </summary>
        /// <typeparam name="TSrc">Тип объекта - источника в регистрации</typeparam>
        /// <typeparam name="TDest">Тип объекта - назначения в регистрации</typeparam>
        /// <param name="source">Объект - источник</param>
        /// <returns>Целевой объект</returns>
        TDest MapTo<TSrc, TDest>(TSrc source);

        /// <summary>
        /// Проекция для LINQ IQueryable - IQueryable
        /// </summary>
        /// <typeparam name="TSrc">Тип объекта - источника</typeparam>
        /// <typeparam name="TDest">Тип целевого объекта</typeparam>
        /// <param name="query">Linq запрос</param>
        /// <returns>Linq запрос с проекцией</returns>
        IQueryable<TDest> ProjectToQuery<TSrc, TDest>(IQueryable<TSrc> query);

        /// <summary>
        /// Проекция для LINQ IQueryable - List
        /// </summary>
        /// <typeparam name="TSrc">Тип объекта - источника</typeparam>
        /// <typeparam name="TDest">Тип целевого объекта</typeparam>
        /// <param name="query">Linq запрос</param>
        /// <returns>Список в памяти в проекции</returns>
        List<TDest> ProjectToList<TSrc, TDest>(IQueryable<TSrc> query);
    }
}
