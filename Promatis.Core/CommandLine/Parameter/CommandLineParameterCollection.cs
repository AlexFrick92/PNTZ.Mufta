using System;
using System.Collections.Generic;
using System.Linq;
using Promatis.Core.Extensions;
using Promatis.Core.Resources;

namespace Promatis.Core.CommandLine
{
    /// <summary>
    /// Коллекция параметров для аргумента
    /// </summary>
    internal class CommandLineParameterCollection
    {
        private readonly Dictionary<string, CommandLineParameter> _parameters;
        private readonly Type _argumentsType;

        /// <summary>
        /// Список значений коллекции
        /// </summary>
        internal CommandLineParameter[] Values => _parameters.Values.ToArray();

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CommandLineParameterCollection"/>
        /// </summary>
        /// <param name="argumentsType">Тип аргументов</param>
        internal CommandLineParameterCollection(Type argumentsType)
        {
            _parameters = new Dictionary<string, CommandLineParameter>();
            _argumentsType = argumentsType;
            Fill();
        }

        /// <summary>
        /// Получает параметр для заданного аргумента
        /// </summary>
        /// <param name="arg">Аргумент</param>
        /// <returns></returns>
        internal CommandLineParameter Get(CommandLineArgument arg)
        {
            _parameters.TryGetValue(arg.Key, out var parameter);
            return parameter;
        }

        /// <summary>
        /// Проверяет заполнение значений для обязательных параметров
        /// </summary>
        internal void VerifyRequiredArguments()
        {
            foreach (var parameter in Values.Where(p => !p.IsValidValue))
            {
                throw new CommandLineException(string.Format(Localization.CommandLine_RequiredArgumentIsNotSet, parameter.Key));
            }
        }

        /// <summary>
        /// Добавляет параметр в коллекцию
        /// </summary>
        /// <param name="parameter">Параметр</param>
        private void Add(CommandLineParameter parameter)
        {
            try
            {
                _parameters.Add(parameter.Key, parameter);
            }
            catch (ArgumentException exception)
            {
                throw new CommandLineException(string.Format(Localization.CommandLine_DuplicateArgument, parameter.Key), exception);
            }
        }

        /// <summary>
        /// Заполняет коллекцию параметров для атрибута
        /// </summary>
        private void Fill()
        {
            var parameters = GetParametersByType(_argumentsType);
            VerifyPositionalArgumentsInSequence();
            parameters.ForEach(Add);
        }

        /// <summary>
        /// Получает список <see cref="CommandLineParameter"/> для всех членов типа аргументов, помеченных атрибутом <see cref="CommandLineParameterAttribute"/>
        /// </summary>
        /// <param name="argumentType">Тип аргументов</param>
        private CommandLineParameter[] GetParametersByType(Type argumentType)
        {
            return argumentType.GetProperties().Where(
                p => p.HasAttribute<CommandLineParameterAttribute>()).Select(p => new CommandLineParameter(p, p.GetAttribute<CommandLineParameterAttribute>())).ToArray();
        }

        /// <summary>
        /// Проверяет последовательность позиций
        /// </summary>
        private void VerifyPositionalArgumentsInSequence()
        {
            var parameters = _parameters.Values.OrderBy(x => x.Position).ToArray();

            for (var i = 0; i < parameters.Length; i++)
            {
                var expectedIndex = i + 1;
                if (parameters[i].Position != expectedIndex)
                {
                    throw new CommandLineException(string.Format(Localization.CommandLine_InvalidPositionsSequence,
                        parameters[i].Key,
                        parameters[i].Position));
                }
            }
        }
    }
}
