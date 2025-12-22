using System;

namespace Promatis.Core.Conversion
{
    /// <summary>
    /// Базовый класс автоматического конвертера объекта одного типа в объект другого типа
    /// </summary>
    /// <typeparam name="TSrc">Тип исходного объекта</typeparam>
    /// <typeparam name="TDest">Тип целевого объекта</typeparam>
    public abstract class AutoConverterBase<TSrc, TDest> : IAutoConverter
    {
        /// <summary>
        /// Инициализирует конвертер
        /// </summary>
        public void Init(IConversionRulesFactory configuration)
        {
            RegisterRules(configuration.CreateDefaultRules<TSrc,TDest>());
        }

        /// <inheritdoc />
        public Type SourceType { get; } = typeof(TSrc);

        /// <inheritdoc />
        public Type DestinationType { get; } = typeof(TDest);

        /// <summary>
        /// Регистрирует уникальные правила конвертации для конкретного типа
        /// <para>Для регистрации правил, необходимо переопределить метод, и внутри него к параметру <paramref name="rules"/> добавлять требуемые правила</para> 
        /// </summary>
        /// <param name="rules">Правила конвертации</param>
        protected virtual void RegisterRules(IConvertionRules<TSrc, TDest> rules)
        {
        }
    }
}
