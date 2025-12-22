using System;
using System.Collections.Generic;
using System.Reflection;
using Promatis.Core.Extensions;

namespace Promatis.Core.Attributes
{
    /// <summary>
    /// Вспомогательные класс, описывающий методы работы с атрибутами
    /// </summary>
    public static class AttributeHelper
    {
        /// <summary>
        /// Получает коллекцию типов из сборки assemblyName, помеченных атрибутом T
        /// </summary>
        /// <typeparam name="T">Атрибут</typeparam>
        /// <param name="assemblyName">Имя сборки</param>
        /// <returns>Коллекция типов</returns>
        public static IList<Type> GetTypes<T>(string assemblyName) where T : Attribute
        {
            var assembly = Assembly.LoadFrom(assemblyName);
            return assembly.GetTypesWithAttribute<T>();
        }
    }
}
