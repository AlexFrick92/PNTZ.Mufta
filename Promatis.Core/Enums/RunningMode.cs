using System;

namespace Promatis.Core
{
    /// <summary>
    /// Режим запуска приложения
    /// </summary>
    [Flags]
    public enum AppRunningMode
    {
        /// <summary>
        /// Тестовая среда
        /// </summary>
        Testing = 0,

        /// <summary>
        /// Боевая среда
        /// </summary>
        Production = 1
    }
}
