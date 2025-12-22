using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Promatis.Core.Extensions
{
    /// <summary>
    /// Методы расширения для типа <see cref="Assembly"/>
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Получает коллекцию типов из сборки, у которых установлен атрибут заданого типа
        /// </summary>
        /// <typeparam name="T">Тип атрибута</typeparam>
        /// <param name="assembly">Текущяа сборка</param>
        /// <returns>Коллекция типов</returns>
        public static IList<Type> GetTypesWithAttribute<T>(this Assembly assembly) where T : Attribute
        {
            Guard.IsNotNull(assembly);
            return assembly.GetTypes().Where(t => t.HasAttribute<T>()).ToList();
        }

        /// <summary>
        /// Получает информацию о версии сборки
        /// </summary>
        /// <param name="assembly">Текущая сборка</param>
        /// <returns></returns>
        public static string GetInformationalVersion(this Assembly assembly)
        {
            Guard.IsNotNull(assembly);
            return assembly
                .GetCustomAttributes<AssemblyInformationalVersionAttribute>()
                .Single()
                .InformationalVersion;
        }

        /// <summary>
        /// Получает коллекцию всех доступных и загруженных типов сборки
        /// </summary>
        /// <param name="assembly">Текущая сборка</param>
        /// <returns></returns>
        public static IEnumerable<Type> GetAccessibleTypes(this Assembly assembly)
        {
            Guard.IsNotNull(assembly);
            try
            {
                return assembly.DefinedTypes.Select(t => t.AsType());
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(t => t != null);
            }
        }


        /// <summary>
        /// Получает имя файла с расширением, соответствующее текущей сборке
        /// </summary>
        /// <param name="assembly">Текущая сборка</param>
        /// <returns></returns>
        public static string GetFileName(this Assembly assembly)
        {
            Guard.IsNotNull(assembly);
            return Path.GetFileName(assembly.Location);
        }

    }
}
