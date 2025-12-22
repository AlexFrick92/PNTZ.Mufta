using System;

namespace Promatis.Core.Conversion
{
    /// <summary>
    /// Интерфейс, описывающий тип автоматического конвертера объекта одного типа в объект другого типа
    /// </summary>
    public interface IAutoConverter
    {
        /// <summary>
        /// Инициализирует конвертер
        /// </summary>
        /// <param name="rulesFactory">Фабрика по созданию правил конвертации</param>
        void Init(IConversionRulesFactory rulesFactory);

        /// <summary>
        /// Тип исходного объекта 
        /// </summary>
        Type SourceType { get; }

        /// <summary>
        /// Тип целевого объекта
        /// </summary>
        Type DestinationType { get; }
    }
}
