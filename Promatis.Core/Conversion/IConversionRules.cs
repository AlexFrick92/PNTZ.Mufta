using System;
using System.Linq.Expressions;

namespace Promatis.Core.Conversion
{
    /// <summary>
    /// Интерфейс правил конвертации для автоматического конвертера
    /// </summary>
    /// <typeparam name="TSrc">Тип исходного объекта</typeparam>
    /// <typeparam name="TDest">Тип целевого объекта</typeparam>
    public interface IConvertionRules<TSrc, TDest>
    {
        /// <summary>
        /// Регистрирует маппинг для свойства
        /// </summary>
        /// <typeparam name="TProp">Тип свойства, для которого устанавливается правило</typeparam>
        /// <param name="propertySelector">Выражение, указывающее на свойство, для которого устанавливается правило</param>
        /// <param name="converterFunc">Выражение, которое выполнится для маппинга из объекта в свойство</param>
        IConvertionRules<TSrc, TDest> ForMember<TProp>(Expression<Func<TDest, TProp>> propertySelector, Expression<Func<TSrc, TProp>> converterFunc);

        /// <summary>
        /// Регистрирует маппинг свойства
        /// </summary>
        /// <typeparam name="TProp">Тип свойства, для которого устанавливается правило</typeparam>
        /// <param name="propertySelector">Выражение, указывающее на свойство, для которого устанавливается правило</param>
        /// <param name="converterFunc">Выражение, которое выполнится для маппинга из объекта в свойство</param>
        /// <remarks>При использовании проекций скачивает объект, а конвертацию производит после. 
        /// Используется при вызове методов или сложной логики в функции конвертации</remarks>
        IConvertionRules<TSrc, TDest> ForMember<TProp>(Expression<Func<TDest, TProp>> propertySelector, Func<TSrc, TProp> converterFunc);

        /// <summary>
        /// Задает свойство, которое не учитывается при конвертации
        /// </summary>
        /// <typeparam name="TProp">Тип свойства, для которого устанавливается правило</typeparam>
        /// <param name="propertySelector">Выражение, указывающее на свойство, для которого устанавливается правило</param>
        /// <returns></returns>
        /// <remarks>Используется для свойств с пред-, постустановкой. Свойство игнорируется в автоматических тестах.
        /// Автоматическая конвертация, в случае возможности ее выполеения, также не будет выполнена</remarks>
        IConvertionRules<TSrc, TDest> IgnoreMember<TProp>(Expression<Func<TDest, TProp>> propertySelector);

        /// <summary>
        /// Игнорирует все остальные свойста при конвертации
        /// </summary>
        /// <returns></returns>
        /// <remarks>Все свойства, которые не были упомянуты в правилах конвертации типов, будут исключены из конвертации</remarks>
        IConvertionRules<TSrc, TDest> IgnoreAllOtherMember();

        /// <summary>
        /// Задает фиксированное значение, которое будет установлено свойству при конвертации
        /// </summary>
        /// <typeparam name="TProp">Тип свойства, для которого устанавливается правило</typeparam>
        /// <param name="propertySelector">Выражение, указывающее на свойство, для которого устанавливается правило</param>
        /// <param name="value">Литеральное значение, которое будет установлено</param>
        /// <returns></returns>
        IConvertionRules<TSrc, TDest> SetValue<TProp>(Expression<Func<TDest, TProp>> propertySelector, TProp value);

        /// <summary>
        /// Действие, которое будет выполнено перед конвертацией
        /// </summary>
        /// <param name="action">Делегат, который необходимо выполнить</param>
        /// <returns></returns>
        IConvertionRules<TSrc, TDest> Before(Action<TSrc, TDest> action);

        /// <summary>
        /// Действие, которое будет выполнено после конвертации свойств
        /// </summary>
        /// <param name="action">Делегат, который необходимо выполнить</param>
        /// <returns></returns>
        IConvertionRules<TSrc, TDest> After(Action<TSrc, TDest> action);
    }
}
