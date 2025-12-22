using System;

namespace Promatis.Core.Integration
{
    /// <summary>
    /// Настройки интеграционной шины
    /// </summary>
    [Serializable]
    public class IntegrationBusSettings
    {
        /// <summary>
        /// Адрес хоста сервера шины
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Интервал проверки подключения в секундах
        /// </summary>
        public TimeSpan HeartBeatInterval { get; set; }

        /// <summary>
        /// Максимальное количество событий в очереди
        /// </summary>
        public int EventsQueueMaxCount { get; set; }

        /// <summary>
        /// Максимальное количество событий в "мертвой" очереди
        /// </summary>
        public int DeadEventsQueueMaxCount { get; set; }

        /// <summary>
        /// Максимальное количество запросов в очереди
        /// </summary>
        public int RequestsMaxCount { get; set; }

        /// <summary>
        /// Время жизни запроса в очереди, в сек.
        /// </summary>
        public int RequestTTL { get; set; }
    }
}
