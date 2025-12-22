namespace Promatis.Core.CommandLine
{
    /// <summary>
    /// Интерфейс конфигурации командной строки
    /// </summary>
    public interface ICommandLineConfiguration
    {
        /// <summary>
        /// Список разделителей аргументов
        /// </summary>
        string[] ArgumentSeparators { get; }

        /// <summary>
        /// Список разделителей значений аргументов
        /// </summary>
        string[] ValueSeparators { get; }

        /// <summary>
        /// Признак чувствительности к регистру
        /// </summary>
        bool CaseSensitive { get; }
    }
}
