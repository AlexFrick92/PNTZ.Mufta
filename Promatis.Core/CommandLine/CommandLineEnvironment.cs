using System;
using System.Linq;

namespace Promatis.Core.CommandLine
{
    /// <summary>
    /// Базовые настройки окружения
    /// </summary>
    internal class CommandLineEnvironment
    {
        private static string[] _args;

        internal static string CommandLine => Environment.CommandLine;

        /// <summary>
        /// Получает список аргументов командной строки
        /// </summary>
        /// <remarks>Первым элементов списка является наименования вызывающего процесса</remarks>
        /// <returns></returns>
        internal static string[] GetCommandLineArgs() => _args ?? (_args = Environment.GetCommandLineArgs());

        /// <summary>
        /// Наименование процесса
        /// </summary>
        internal static string Program => GetCommandLineArgs()[0].Split('\\').LastOrDefault();

        /// <summary>
        /// Получает список разделителей аргументов
        /// </summary>
        internal static string[] ArgumentSeparators { get; private set; } = { "/", "-" };

        /// <summary>
        /// Получает список разделителей значений аргументов
        /// </summary>
        internal static string[] ValueSeparators { get; private set; } = {":", "="};

        /// <summary>
        /// Получает список разделителей аргументов
        /// </summary>
        internal static string[] HelpCodes { get; private set; } = { "?", "help" };

        /// <summary>
        /// Выражение для разбора токена аргумента
        /// </summary>
        internal const string TokenizeExpressionFormat =
            @"(?{0}i) # Управление регистром
            (?<SwitchSeparator>\A[{1}]) # Разделитель аргументов от начала строки 
            (?<SwitchName>[^{2}+-]+) # Имя аргумента
            (?<SwitchOption>[{2}+-]|\z) # Параметры аргумента
            (?<Value>.*)\Z # Значение аргумента ";

        /// <summary>
        /// Признак чувствительности к регистру
        /// </summary>
        internal static bool CaseSensitive { get; private set; }

        /// <summary>
        /// Применяет настройки заданной конфигурации
        /// </summary>
        /// <param name="configuration">УКонфигурация</param>
        internal static void ApplyConfiguration(ICommandLineConfiguration configuration)
        {
            ArgumentSeparators = configuration.ArgumentSeparators;
            ValueSeparators = configuration.ValueSeparators;
            CaseSensitive = configuration.CaseSensitive;
        }

        /// <summary>
        /// Получает ключ параметра по его позиции
        /// </summary>
        /// <param name="position">Позиция</param>
        /// <returns></returns>
        internal static string GetParameterKey(int position) => $"parameter[{position}]";
    }
}
