using System;
using System.Linq;

namespace Promatis.Core.CommandLine
{
    /// <summary>
    /// Атрибут параметра командной строки
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CommandLineParameterAttribute : Attribute
    {
        /// <summary>
        /// Ключ
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Значение по умолчанию
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Признак обязательности параметра
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Пример значения
        /// </summary>
        public string ValueExample { get; set; }

        /// <summary>
        /// Позиция параметра
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Признак указывающий на то, что параметр является аргументом (т.е. задается без указания ключа)
        /// </summary>
        public bool IsArg { get; set; }

        /// <summary>
        /// Форматированое описание
        /// </summary>
        /// <returns></returns>
        public string FormattedArgumentString => string.Format(Required ? "{0}{1}" : "[{0}{1}]",
            IsArg ? string.Empty : CommandLineEnvironment.ArgumentSeparators.FirstOrDefault(), Key);

    }
}
