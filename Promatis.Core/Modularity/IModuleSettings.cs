using System;

namespace Promatis.Core.Modularity
{
    /// <summary>
    /// Интерфейс, описывающий настройки модуля
    /// </summary>
    [Obsolete("Устарел. Заменен на Promatis.ServiceModel.Interface.IApplicationSettings")]
    public interface IModuleSettings
    {
        /// <summary>
        /// Наименование модуля
        /// </summary>
        string ModuleName { get; }

        /// <summary>
        /// Каталог внутрисистемных служб модуля
        /// </summary>
        IInternalServiceCatalog InternalServicesCatalog { get; }

        /// <summary>
        /// Режим запуска модуля
        /// </summary>
        AppRunningMode RunningMode { get; }
    }
}