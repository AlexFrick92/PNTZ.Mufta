using System;
using System.Linq;
using System.Reflection;

namespace Promatis.Core.Extensions
{
    /// <summary>
    /// Методы расширения для типа <see cref="PropertyInfo"/>
    /// </summary>
    public static class PropertyInfoExtensions
    {
        /// <summary>
        /// Получает атрибут заданного типа у текущего свойства
        /// </summary>
        /// <typeparam name="T">Тип атрибута</typeparam>
        /// <param name="info">Текущее свойство</param>
        /// <returns>Экземпляр <typeparamref name="T"/>. Если атриьута такого типа нет, то <c>null</c></returns>
        public static T GetAttribute<T>(this PropertyInfo info) where T : Attribute
        {
            var attributes = info.GetCustomAttributes(typeof(T), true);
            return attributes.Any() ? (T)attributes[0] : null;
        }

        /// <summary>
        /// Проверяет наличие у свойства атрибута заданного типа
        /// </summary>
        /// <typeparam name="T">Тип атрибута</typeparam>
        /// <param name="info">Текущее свойство</param>
        /// <returns>Если атрибут присутствует, то <c>true</c>, иначе <c>false</c></returns>
        public static bool HasAttribute<T>(this PropertyInfo info) where T : Attribute
        {
            return info.GetCustomAttributes(typeof(T), true).Any();
        }
    }
}
