namespace Promatis.Core.Conversion
{
    /// <summary>
    /// Интерфейс фабрики по созданию правил конвертации
    /// </summary>
    public interface IConversionRulesFactory
    {
        /// <summary>
        /// Создает правила конвертации по умолчанию
        /// </summary>
        /// <typeparam name="TSrc">Тип исходного объекта</typeparam>
        /// <typeparam name="TDest">Тип целевого объекта</typeparam>
        /// <returns></returns>
        IConvertionRules<TSrc, TDest> CreateDefaultRules<TSrc, TDest>();
    }
}