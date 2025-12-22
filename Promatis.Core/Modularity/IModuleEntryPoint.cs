using Promatis.Core.Integration;
using Promatis.Core.Logging;
using System;

namespace Promatis.Core.Modularity
{
    /// <summary>
    /// Интерфейс входной точки модуля
    /// </summary>
    [Obsolete("Устарел. Заменен на Promatis.ServiceModel.Interface.IApplicationBuilder")]
    public interface IModuleEntryPoint
    {
        /// <summary>
        /// Выполняет запуск модуля
        /// </summary>
        /// <param name="mode">Режим запуска</param>
        void Run(AppRunningMode mode);

        /// <summary>
        /// Выполняет остановку модуля
        /// </summary>
        void Stop();

        /// <summary>
        /// Выполняет инициализацию модуля на основе заданной конфигурации с логированием
        /// </summary>
        /// <param name="settings">Конфигурация модуля</param>
        /// <param name="logger">Экземпляр логгера</param>
        void Initialize(IModuleSettings settings, ILogger logger);

        /// <summary>
        /// Выполняет настройку интеграции модуля через интеграционную шину
        /// </summary>
        /// <param name="integrationBus">Интеграционная шина</param>
        /// <param name="logger">Экземпляр логгера</param>
        void SetupIntegration(IIntegrationBus integrationBus, ILogger logger);

        /// <summary>
        /// Настраивает контейнер инверсии зависимостей
        /// </summary>
        /// <param name="container">Экземпляр DI контейнера</param>
        /// <param name="logger">Экземпляр логгера</param>
        void ConfigureDependencyInjection(IIoCContainer container, ILogger logger);
    }
}