using System;

namespace Promatis.Core.Interfaces
{
    /// <summary>
    /// Интерфейс объекта, идентифицируемого по глобальному идентификатору GUID
    /// </summary>
    public interface IIdentifiedByGuid
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        Guid UId { get; }
    }
}
