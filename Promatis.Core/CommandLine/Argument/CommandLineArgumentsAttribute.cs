using System;
using System.Reflection;

namespace Promatis.Core.CommandLine
{
    /// <summary>
    /// Атрибут класса аргументов командной строки
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandLineArgumentsAttribute : Attribute
    {
        /// <summary>
        /// Заголовок при выводе справки по программе
        /// </summary>
        public string Title => GetFormattedTitleText();

        /// <summary>
        /// Описание программы
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Формирует строку заголовка
        /// </summary>
        /// <returns></returns>
        private string GetFormattedTitleText()
        {
            var assembly = Assembly.GetEntryAssembly();
            return assembly != null
                ? $"{assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title} {assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright}"
                : string.Empty;
        }

        
    }
}
