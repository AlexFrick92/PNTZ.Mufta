using System.Collections.Generic;

namespace Promatis.Core.Integration
{
    /// <summary>
    /// Интерфейс конвертера объекта в коллекцию сообщений для передачи в интеграционную шину
    /// </summary>
    /// <typeparam name="TEntity">Тип объекта</typeparam>
    /// <typeparam name="TMessage">Тип сообщения</typeparam>
    public interface IMessageCollectionConverter<in TEntity, TMessage>
    {
        /// <summary>
        /// Конвертирует заданый объект в коллекцию сообщений интеграционной шины
        /// </summary>
        /// <param name="entity">Объект</param>
        /// <returns>Коллекция сообщений</returns>
        IList<TMessage> ConvertToMessages(TEntity entity);
    }
}