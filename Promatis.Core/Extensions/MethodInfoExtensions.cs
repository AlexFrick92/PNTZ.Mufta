using System;
using System.Linq;
using System.Reflection;

namespace Promatis.Core.Extensions
{
    /// <summary>
    /// Методы расширения для типа <see cref="MethodInfo"/>
    /// </summary>
    public static class MethodInfoExtensions
    {
        /// <summary>
        /// Получает атрибут заданного типа у текущего метода
        /// </summary>
        /// <typeparam name="T">Тип атрибута</typeparam>
        /// <param name="info">Текущий метод</param>
        /// <returns>Экземпляр <typeparamref name="T"/>. Если атрибута такого типа нет, то <c>null</c></returns>
        public static T GetAttribute<T>(this MethodInfo info) where T : Attribute
        {
            var attributes = info.GetCustomAttributes(typeof(T), true);
            return attributes.Any() ? (T)attributes[0] : null;
        }

        /// <summary>
        /// Получает коллекцию атрибутов заданного типа у текущего метода
        /// </summary>
        /// <typeparam name="T">Тип атрибута</typeparam>
        /// <param name="info">Текущий метод</param>
        /// <returns>Коллекция экземпляров <typeparamref name="T"/>. Если атрибута такого типа нет, то <c>null</c></returns>
        public static T[] GetAttributes<T>(this MethodInfo info) where T : Attribute
        {
            var attributes = info.GetCustomAttributes(typeof(T), true);
            return attributes.Any() ? (T[])attributes : null;
        }

        /// <summary>
        /// Проверяет наличие у метода атрибута заданного типа
        /// </summary>
        /// <typeparam name="T">Тип атрибута</typeparam>
        /// <param name="info">Текущий метод</param>
        /// <returns>Если атрибут присутствует, то <c>true</c>, иначе <c>false</c></returns>
        public static bool HasAttribute<T>(this MethodInfo info) where T : Attribute
        {
            return info.GetCustomAttributes(typeof(T), true).Any();
        }


    }
}
