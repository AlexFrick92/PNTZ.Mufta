using System;
using System.Reflection;

namespace Promatis.Core.Conversion
{
    /// <summary>
    /// Интерфейс менеджера конвертации объектов одного  типа в объекты другого типа
    /// </summary>
    public interface IConversionManager
    {
        /// <summary>
        /// Сервис конвертации
        /// </summary>
        IConversionService ConversionService { get; }

        /// <summary>
        /// Фабрика создания правил конвертации
        /// </summary>
        IConversionRulesFactory RulesFactory { get; }
        
        /// <summary>
        /// Инициализирует все зарегистрированные конвертеры
        /// </summary>
        void InitializeAutoConverters();

        #region [Методы регистрации типов]

        /// <summary>
        /// Регистрирует все типы конвертеров, которые содержатся в заданной сборке
        /// </summary>
        /// <param name="assembly">Сборка с конвертерами</param>
        /// <param name="excludeTypes">Список типов, которые нужно исключить из регистрации</param>
        void Register(Assembly assembly, params Type[] excludeTypes);

        /// <summary>
        /// Регистрирует все типы конверторов указанных в списке
        /// </summary>
        /// <param name="includeTypes">Список типов конвертеров</param>
        void Register(params Type[] includeTypes);

        /// <summary>
        /// Создает и регистрирует конвертер заданного типа
        /// </summary>
        /// <typeparam name="T">Тип конвертера</typeparam>
        void Register<T>();

        /// <summary>
        /// Создает и регистрирует конвертер заданного типа
        /// </summary>
        /// <param name="autoConverterType">Тип конвертера</param>
        void Register(Type autoConverterType);

        #endregion
    }
}