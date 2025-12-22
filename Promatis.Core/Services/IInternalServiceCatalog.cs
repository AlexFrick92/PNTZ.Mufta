using System;

namespace Promatis.Core
{
    /// <summary>
    /// Интерфейс, описывающий каталог запущенных внутрисистемных служб. 
    /// </summary>
    /// <remarks>Все методы должны быть потокобезопасными</remarks>
    /// <threadsafety static="true" instance="true" />
    [Obsolete("Устарел. Заменен на Promatis.ServiceModel.Interface.IInternalServiceCollection")]
    public interface IInternalServiceCatalog
    {
        /// <summary>
        /// Получает конечную точку внутрисистемной службы по её имени.
        /// Потокобезопасный.
        /// </summary>
        /// <param name="serviceName">Имя службы</param>
        /// <returns></returns>
        string Get(string serviceName);
    }
}