
using Promatis.Core.Results;

namespace Promatis.Core.Integration
{
    /// <summary>
    /// Интерфейс интеграционной шины для взаимодействия между модулями
    /// </summary>
    public interface IIntegrationBus
    {
        /// <summary>
        /// Запускает прослушивание интергационной шины
        /// </summary>
        void StartListen();

        /// <summary>
        /// Останавливает прослушивание интеграционной шины
        /// </summary>
        void StopListen();

        /// <summary>
        /// Разрывает соединение с интеграционной шиной
        /// </summary>
        void Disconnect();

        #region [События]

        /// <summary>
        /// Отправляет событие с заданным наименованием
        /// подписчкам
        /// </summary>
        /// <param name="eventName">Наименование события</param>
        /// <param name="eventBody">Сообщение</param>
        /// <param name="ttl">Время жизни события в очереди</param>
        void PublishEvent(string eventName, object eventBody, int ttl);

        /// <summary>
        /// Подписывается на событие по его наименованию
        /// </summary>
        /// <typeparam name="TMsg">Тип сообщения</typeparam>
        /// <param name="eventName">Наименование события</param>
        /// <param name="eventHandler">Обработчик события</param>
        void SubscribeEvent<TMsg>(string eventName, IEventHandler<TMsg> eventHandler);

        #endregion

        #region [Синхронные Запросы]

        /// <summary>
        /// Выполняет rpc вызов к указанному модулю
        /// </summary>
        /// <typeparam name="TOut">Тип результата выполнения запроса</typeparam>
        /// <param name="moduleName">Наименование модуля</param>
        /// <param name="requestName">Наименование запроса</param>
        /// <param name="requestArgs">Параметры запроса</param>
        /// <param name="timeoutMilliseconds">Время ожидания ответа в миллисекундах. По умолчанию <c>500</c></param>
        /// <returns>Результат выполнения запроса</returns>
        TOut InvokeRequest<TOut>(string moduleName, string requestName, object requestArgs, int timeoutMilliseconds = 500);

        /// <summary>
        /// Выполняет rpc вызов к указанному модулю
        /// </summary>
        /// <typeparam name="TOut">Тип результата выполнения запроса</typeparam>
        /// <param name="moduleName">Наименование модуля</param>
        /// <param name="requestName">Наименование запроса</param>
        /// <param name="timeoutMilliseconds">Время ожидания ответа в миллисекундах.</param>
        /// <returns>Результат выполнения запроса</returns>
        TOut InvokeRequest<TOut>(string moduleName, string requestName, int timeoutMilliseconds);

        /// <summary>
        /// Выполняет безопасный rpc вызов к указанному модулю
        /// </summary>
        /// <typeparam name="TOut">Тип результата выполнения запроса</typeparam>
        /// <param name="moduleName">Наименование модуля</param>
        /// <param name="requestName">Наименование запроса</param>
        /// <param name="requestArgs">Параметры запроса</param>
        /// <param name="timeoutMilliseconds">Время ожидания ответа в миллисекундах. По умолчанию <c>500</c></param>
        /// <returns>Результат выполнения запроса</returns>
        OperationResult<TOut> SafeInvokeRequest<TOut>(string moduleName, string requestName, object requestArgs, int timeoutMilliseconds = 500);

        /// <summary>
        /// Выполняет безопасный rpc вызов к указанному модулю
        /// </summary>
        /// <typeparam name="TOut">Тип результата выполнения запроса</typeparam>
        /// <param name="moduleName">Наименование модуля</param>
        /// <param name="requestName">Наименование запроса</param>
        /// <param name="timeoutMilliseconds">Время ожидания ответа в миллисекундах.</param>
        /// <returns>Результат выполнения запроса</returns>
        OperationResult<TOut> SafeInvokeRequest<TOut>(string moduleName, string requestName, int timeoutMilliseconds);

        /// <summary>
        /// Регистрирует обработчик синхронного запроса
        /// </summary>
        /// <typeparam name="TIn">Тип входного параметра</typeparam>
        /// <typeparam name="TOut">Тип результата выполнения</typeparam>
        /// <param name="methodName">Наименование запроса</param>
        /// <param name="requestHandler">Обработчик запроса</param>
        void RegisterRequestHandler<TIn, TOut>(string methodName, IRequestHandler<TIn, TOut> requestHandler);

        /// <summary>
        /// Регистрирует обработчик синхронного запроса
        /// </summary>
        /// <typeparam name="TOut">Тип результата выполнения</typeparam>
        /// <param name="methodName">Наименование запроса</param>
        /// <param name="requestHandler">Обработчик запроса</param>
        void RegisterRequestHandler<TOut>(string methodName, IRequestHandler<TOut> requestHandler);

        #endregion

        #region [Отложенные сообщения]

        /// <summary>
        /// Формирует событие(асинихронный запрос) на отправку и помещает его в очередь, не отправляя немедленно в шину
        /// </summary>
        /// <param name="eventName">Наименование события</param>
        /// <param name="eventBody">Сообщение</param>
        /// <param name="timeToLife">Время жизни события в очереди шины</param>
        void PublishDelayedEvent(string eventName, object eventBody, int timeToLife);

        /// <summary>
        /// Отправляет все отложенные события(асинихронные запросы) в шину
        /// </summary>
        void FlushDelayedEvents();

        /// <summary>
        /// Очищает очередь отложенных событий
        /// </summary>
        void CleanDelayedEvents();

        #endregion
    }
}
