namespace Promatis.Core.Integration
{ 
    /// <summary>
    /// Интерфейс обработчика события(асинихронного запроса) интеграционной шины
    /// </summary>
    /// <typeparam name="T">Тип сообщения в событии</typeparam>
    public interface IEventHandler<in T>
    {
        /// <summary>
        /// Вызывает обработку события
        /// </summary>
        /// <param name="eventName">Наименование события</param>
        /// <param name="message">Сообщение</param>
        void ProcessMessage(string eventName, T message);
    }
}
