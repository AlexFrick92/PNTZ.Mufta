using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Promatis.Core.Extensions;

namespace Promatis.Core.CommandLine
{
    /// <summary>
    /// Справочный аргумент
    /// </summary>
    internal class CommandLineHelp
    {
        private readonly List<CommandLineParameterAttribute> _parameters = new List<CommandLineParameterAttribute>();
        private readonly string _title;
        private readonly string _description;
        private readonly string _programName;
        
        /// <summary>
        /// Сообщение
        /// </summary>
        internal string Message { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CommandLineHelp"/>
        /// </summary>
        /// <param name="argumentsType">Тип аргументов</param>
        internal CommandLineHelp(Type argumentsType)
            : this(argumentsType, string.Empty)
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CommandLineHelp"/>
        /// </summary>
        /// <param name="property">Свойство</param>
        /// <param name="message">Сообщение</param>
        internal CommandLineHelp(PropertyInfo property, string message)
            : this(property.DeclaringType, message)
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CommandLineHelp"/>
        /// </summary>
        /// <param name="argumentsType">Тип аргументов</param>
        /// <param name="message">Сообщение</param>
        internal CommandLineHelp(Type argumentsType, string message)
        {
            var arguments = argumentsType.GetAttribute<CommandLineArgumentsAttribute>();
            _title = arguments?.Title ?? string.Empty;
            _description = arguments?.Description ?? string.Empty;
            _programName = CommandLineEnvironment.Program;
            _parameters.AddRange(GetValidAttributesByType(argumentsType));

            Message = message;
        }

        /// <summary>
        /// Получает отформатированный текст справки
        /// </summary>
        /// <param name="maxWidth">Максимальная ширина</param>
        /// <param name="margin">Отступ</param>
        /// <returns></returns>
        internal string GetHelpText(int maxWidth, int margin)
        {
            var sb = new StringBuilder();
            sb.AppendLine(_title);
            sb.AppendLine(_description);
            sb.AppendLine();

            AppendCommandLineExample(sb, maxWidth, margin, out var maxParameterWidth);

            AppendParametesInfo(sb, maxWidth, maxParameterWidth, margin);
            return sb.ToString();
        }

        /// <summary>
        /// Добавляет в экземпляр <see cref="StringBuilder"/> строку примера использования команды
        /// </summary>
        /// <param name="sb">Экземпляр <see cref="StringBuilder"/></param>
        /// <param name="maxWidth">Максимальная ширина</param>
        /// <param name="margin">Отступ</param>
        /// <param name="maxParameterWidth">Максимальная ширина наименований параметров</param>
        private void AppendCommandLineExample(StringBuilder sb, int maxWidth, int margin, out int maxParameterWidth)
        {
            var paramSb = new StringBuilder();

            maxParameterWidth = 0;
            foreach (var parameterName in _parameters.OrderBy(x => x.Position).Select(p=>p.FormattedArgumentString))
            {
                paramSb.AppendFormat("{0} ", parameterName);
                if (parameterName.Length > maxParameterWidth)
                    maxParameterWidth = parameterName.Length;
            }
            var parms = paramSb.ToString();

            var nameLines = _programName.Wrap(maxWidth);
            var descriptionLines = parms.Wrap(maxWidth - _programName.Length - margin);

            AppendLinesOnTwoColumns(sb, _programName.Length + 1, nameLines, descriptionLines);
            sb.AppendLine();
        }

        /// <summary>
        /// Добавляет в экземпляр <see cref="StringBuilder"/> строки описания параметров
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxParameterWidth"></param>
        /// <param name="margin"></param>
        private void AppendParametesInfo(StringBuilder sb, int maxWidth, int maxParameterWidth, int margin)
        {
            foreach (var parameter in _parameters.OrderBy(x => x.Position))
            {
                var nameLines = parameter.FormattedArgumentString.Wrap(maxParameterWidth + margin);
                var descriptionLines = (parameter.Description ?? string.Empty).Wrap(maxWidth - maxParameterWidth - margin);

                AppendLinesOnTwoColumns(sb, maxParameterWidth, nameLines, descriptionLines, margin);
            }
        }

        /// <summary>
        /// Получает список <see cref="CommandLineParameter"/> для всех членов типа аргументов, помеченных атрибутом <see cref="CommandLineParameterAttribute"/>
        /// </summary>
        /// <param name="argumentType">Тип аргументов</param>
        private CommandLineParameterAttribute[] GetValidAttributesByType(Type argumentType)
        {
            return argumentType.GetProperties().Where(
                p => p.HasAttribute<CommandLineParameterAttribute>()).Select(p => p.GetAttribute<CommandLineParameterAttribute>()).ToArray();
        }

        /// <summary>
        /// Добавляет строки в виде двух колонок
        /// </summary>
        /// <param name="sb">Экземпляр <see cref="StringBuilder"/></param>
        /// <param name="firstColumnWidth">Ширина первой колонки</param>
        /// <param name="firstColumn">Список значений первой колонки</param>
        /// <param name="secondColumn">Список значений второй колонки</param>
        /// <param name="margin">Отступ первой колонки</param>
        private void AppendLinesOnTwoColumns(StringBuilder sb, int firstColumnWidth, IList<string> firstColumn, IList<string> secondColumn, int margin = 0)
        {
            var format = $"{{2,{margin}}}{{0,-{firstColumnWidth + 1}}}{{1}}";
            for (var i = 0; i < firstColumn.Count || i < secondColumn.Count; i++)
            {
                sb.AppendLine(
                    string.Format(
                        format, i < firstColumn.Count ? firstColumn[i] : string.Empty,
                        i < secondColumn.Count ? secondColumn[i] : string.Empty,
                        margin>0 ? " " : string.Empty));
            }
        }

    }
}
