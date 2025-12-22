namespace Promatis.Core.Integration
{
    /// <summary>
    /// Интерфейс конвертера объекта в сообщение для передачи в шину
    /// </summary>
    /// <typeparam name="TEntity">Тип объекта</typeparam>
    /// <typeparam name="TMessage">Тип сообщения</typeparam>
    public interface IMessageConverter<in TEntity, out TMessage>
    {
        /// <summary>
        /// Конвертирует заданый объект в сообщение интеграционной шины
        /// </summary>
        /// <param name="entity">Исходный объект</param>
        /// <returns>Сообщение шины</returns>
        TMessage Convert(TEntity entity);
    }
}
