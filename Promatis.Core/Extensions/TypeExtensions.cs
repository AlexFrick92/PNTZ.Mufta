using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// ReSharper disable BuiltInTypeReferenceStyle

namespace Promatis.Core.Extensions
{
    /// <summary>
    /// Методы расширения для типа <see cref="Type"/>
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Получает атрибут заданного типа у текущего типа
        /// </summary>
        /// <typeparam name="T">Тип атрибута</typeparam>
        /// <param name="type">Текущий тип</param>
        /// <returns>Экземпляр <typeparamref name="T"/>. Если атрибута такого типа нет, то <c>null</c></returns>
        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            var attributes = type.GetCustomAttributes(typeof(T), true);
            return attributes.Any() ? (T)attributes[0] : null;
        }

        /// <summary>
        /// Получает коллекцию атрибутов заданного типа у текущего типа
        /// </summary>
        /// <typeparam name="T">Тип атрибута</typeparam>
        /// <param name="type">Текущий тип</param>
        /// <returns>Коллекция экземпляров <typeparamref name="T"/>. Если атрибута такого типа нет, то <c>null</c></returns>
        public static IEnumerable<T> GetAttributes<T>(this Type type) where T : Attribute
        {
            var attributes = type.GetCustomAttributes(typeof(T), true);
            return attributes.Any() ? (IEnumerable<T>)attributes.AsEnumerable() : null;
        }

        /// <summary>
        /// Проверяет наличие у текущего типа атрибута заданного типа
        /// </summary>
        /// <typeparam name="T">Тип атрибута</typeparam>
        /// <param name="type">Текущий тип</param>
        /// <returns>Если атрибут присутствует, то <c>true</c>, иначе <c>false</c></returns>
        public static bool HasAttribute<T>(this Type type) where T : Attribute => type.GetCustomAttributes(typeof(T), true).Any();

        /// <summary>
        /// Определяет, является ли текущий тип простым (т.е. тип может быть сериализован как и был)
        /// </summary>
        /// <para>Типы: Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, Char, Double, Single
        /// Decimal, Guid, DateTime, String, Enum</para>
        /// <param name="type">Текущий тип</param>
        /// <returns></returns>
        public static bool IsSimpleType(this Type type)
        {
            return type.IsStringType() ||
                   type.IsPrimitive ||
                   type.IsDecimalType() ||
                   type == typeof(Guid) ||
                   type == typeof(DateTime) ||
                   type.IsEnum;
        }

        /// <summary>
        /// Определяет, является ли текущий тип простым nullable
        /// </summary>
        /// <param name="type">Текущий тип</param>
        /// <returns></returns>
        public static bool IsNullableSimpleType(this Type type)
        {
            return type.IsGenericType &&
                   type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                   type.GetGenericArguments()[0].IsSimpleType();
        }

        /// <summary>
        /// Определяет, является ли текущий тип List{} или IList{}
        /// </summary>
        /// <param name="type">Текущий тип</param>
        /// <returns></returns>
        public static bool IsListType(this Type type)
        {
            return type.IsGenericType &&
                   (type.GetGenericTypeDefinition() == typeof(IList<>) ||
                    type.GetGenericTypeDefinition() == typeof(List<>));
        }

        /// <summary>
        /// Определяет, является ли текущий тип String
        /// </summary>
        /// <param name="type">Текущий тип</param>
        /// <returns></returns>
        public static bool IsStringType(this Type type) => type == typeof(string) || type == typeof(String);

        /// <summary>
        /// Определяет, является ли текущий тип Decimal
        /// </summary>
        /// <param name="type">Текущий тип</param>
        /// <returns></returns>
        public static bool IsDecimalType(this Type type) => type == typeof(decimal) || type == typeof(Decimal);

        /// <summary>
        /// Определяет, является ли текущий тип числовым
        /// </summary>
        /// <para>Типы: Int16, UInt16, Int32, UInt32, Int64, UInt64, Double, Decimal, Float и из nullable версии</para>
        /// <param name="type">Текущий тип</param>
        /// <returns></returns>
        public static bool IsDigitalType(this Type type)
        {
            return type == typeof(decimal) || type == typeof(Decimal) ||
                   type == typeof(float) ||
                   type == typeof(double) || type == typeof(Double) ||
                   type == typeof(short) || type == typeof(Int16) ||
                   type == typeof(int) || type == typeof(Int32) ||
                   type == typeof(long) || type == typeof(Int64) ||
                   type == typeof(ushort) || type == typeof(UInt16) ||
                   type == typeof(uint) || type == typeof(UInt32) ||
                   type == typeof(ulong) || type == typeof(UInt64) ||

                   type == typeof(decimal?) || type == typeof(Decimal?) ||
                   type == typeof(float?) ||
                   type == typeof(double?) || type == typeof(Double?) ||
                   type == typeof(short?) || type == typeof(Int16?) ||
                   type == typeof(int?) || type == typeof(Int32?) ||
                   type == typeof(long?) || type == typeof(Int64?) ||
                   type == typeof(ushort?) || type == typeof(UInt16?) ||
                   type == typeof(uint?) || type == typeof(UInt32?) ||
                   type == typeof(ulong?) || type == typeof(UInt64?);
        }

        /// <summary>
        /// Получает экземплярный конструктор типа с сигнатурой, в которой типы параметров и их последовательность совпадают с переданными
        /// </summary>
        /// <param name="type">Текущий тип</param>
        /// <param name="parameterTypes">Типы параметров</param>
        /// <returns></returns>
        public static ConstructorInfo GetDeclaredConstructor(this Type type, params Type[] parameterTypes)
        {
            return type.GetTypeInfo().DeclaredConstructors.SingleOrDefault(
                c => !c.IsStatic && c.GetParameters().Select(p => p.ParameterType).SequenceEqual(parameterTypes));
        }

        /// <summary>
        /// Получает открытый экземплярный конструктор типа с сигнатурой, в которой типы параметров и их последовательность совпадают с переданными
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        public static ConstructorInfo GetPublicConstructor(this Type type, params Type[] parameterTypes)
        {
            var constructor = type.GetDeclaredConstructor(parameterTypes);

            return constructor != null && constructor.IsPublic ? constructor : null;
        }

    }
}
