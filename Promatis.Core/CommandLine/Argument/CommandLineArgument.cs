using Promatis.Core.Extensions;
using System;
using System.Text.RegularExpressions;

namespace Promatis.Core.CommandLine
{
    /// <summary>
    /// Аргумент командной строки
    /// </summary>
    [Serializable]
    internal class CommandLineArgument
    {
        private string _value;
        private readonly string _name;
        private readonly string _token;
        private readonly int _position = -1;

        /// <summary>
        /// Инициализирует новый экземпяр <see cref="CommandLineArgument"/>
        /// </summary>
        /// <param name="match">Регулярное выражение</param>
        internal CommandLineArgument(Match match)
        {
            _token = GetGroupValue(match, 0);
            _name = GetGroupValue(match, "SwitchName");
            _value = GetGroupValue(match, "Value");
        }

        /// <summary>
        /// Инициализирует новый экземпяр <see cref="CommandLineArgument"/>
        /// </summary>
        /// <param name="token">Токен аргумента</param>
        /// <param name="parameterPosition">Индекс</param>
        internal CommandLineArgument(string token, int parameterPosition)
        {
            _token = token;
            _position = parameterPosition;
        }

        /// <summary>
        /// Значение аргумента
        /// </summary>
        internal string Value
        {
            get => _value.IsEmpty() && _position != -1 ? _token : _value;
            set => _value = value;
        }
        
        /// <summary>
        /// Ключ аргумента
        /// </summary>
        /// <remarks>Если наименование аргумента не указано, то принимает значение вида <c>Parameter[Position]</c>,
        /// иначе -  наименование аргумента </remarks>
        internal string Key => _name.IsEmpty()
            ? CommandLineEnvironment.GetParameterKey(_position)
            : CommandLineEnvironment.CaseSensitive ? _name : _name.ToLowerInvariant();


        private string GetGroupValue(Match match, string group)
        {
            return match.Groups[group].Success
                       ? match.Groups[group].Value.Trim()
                       : null;
        }

        private string GetGroupValue(Match match, int group)
        {
            return match.Groups[group].Success
                       ? match.Groups[group].Value.Trim()
                       : null;
        }
    }
}
