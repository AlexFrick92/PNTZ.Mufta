using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace Promatis.Core.Extensions
{
    /// <summary>
    /// Методы расширения для типа <see cref="Enum"/>
    /// </summary>
    public static class EnumExtensions
    {
        private static readonly ConcurrentDictionary<Assembly, ResourceManager> ResourceManagers = new ConcurrentDictionary<Assembly, ResourceManager>();

        #region Description

        /// <summary>
        /// Получает значение атрибута <c>Descriptions</c>, либо если его нет то строковое значение элемента
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetDescription<TEnum>(this TEnum enumValue)
        {
            if (enumValue == null)
                return string.Empty;
            var enumType = enumValue.GetType();
            var enumName = enumValue.ToString();

            var enumMember = enumType.GetMember(enumName);
            if (enumMember.Length > 0)
            {
                var enumAttributes = enumMember[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (enumAttributes.Length > 0)
                    return ((DescriptionAttribute)enumAttributes[0]).Description;
            }
            return enumName;
        }

        /// <summary>
        /// Получает локализованное описание для значения перечисления
		/// <para>(вызывает исключение если не найдена или не реализована локализация для данного значения)</para>
        /// </summary>
        /// <typeparam name="TEnum">тип перечисления</typeparam>
        /// <param name="enumValue">Значение перечисления</param>
        /// <returns></returns>
		public static string GetLocalizedValue<TEnum>(this TEnum enumValue) where TEnum : struct
        {
            var assembly = typeof(TEnum).Assembly;
            if (!ResourceManagers.TryGetValue(assembly, out var resourceManager))
            {
                var localizationType = assembly.GetTypes().FirstOrDefault(p => p.Name == "Localization");
                Guard.IsNotNull(localizationType, "localizationType",
                    $"Localization type not found in {assembly}");

                var propertyInfo = localizationType?.GetProperty("ResourceManager", BindingFlags.Static | BindingFlags.NonPublic);
                resourceManager = propertyInfo.SafeGet(p => p.GetValue(null, null) as ResourceManager);
                Guard.IsNotNull(resourceManager, "resourceManager",
                    $"Property ResourceManager not found in type {localizationType}");

                ResourceManagers.TryAdd(assembly, resourceManager);
            }
            var description = resourceManager.GetString(
                $"{typeof(TEnum).Name}_{Enum.GetName(typeof(TEnum), enumValue)}");
            Guard.IsNotNull(description, "description",
                $"Not found Localized description '{typeof(TEnum).Name}_{Enum.GetName(typeof(TEnum), enumValue)}'");

            return description;
        }

        /// <summary>
        /// Получает описание, определенное в атрибуте <c>DisplayStringAttribute</c>.
        /// <para>Если атрибут не определен, возвращается значение по умолчанию полученное методом <c>ToString()</c></para>
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Описание, определенное в атрибуте <c>DisplayStringAttribute</c>. </returns>
        public static string GetDisplayString(this Enum value)
        {
            FieldInfo info = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])info.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length >= 1 ? attributes[0].Description : value.ToString();
        }

        #endregion

        #region Flags

        /// <summary>
        /// Проверяет вхождение ЛЮБОГО ИЗ значений в перечисление
        /// </summary>
        public static bool HasAnyFlag(this Enum thisEnum, Enum flags)
        {
            var enumType = thisEnum.GetType();
            CheckFlagsAttribute(enumType);

            if (enumType != flags.GetType())
                throw new ArgumentException("Тип перечисления-параметра не совпадает типом перечисления, для которого вызывается функция", nameof(flags));

            var flagValue = Convert.ToInt64(flags);
            return (Convert.ToInt64(thisEnum) & flagValue) != 0;
        }

        /// <summary>
        /// Проверяет наличие всех возможных значений перечисления в объекте
        /// </summary>
        /// <param name="thisEnum">Перечисление</param>
        /// <returns>true, если объект содержит все значения перечисления, иначе false</returns>
        public static bool HasAllFlags(this Enum thisEnum)
        {
            var enumType = thisEnum.GetType();
            CheckFlagsAttribute(enumType);

            var flagPow = Enum.GetNames(enumType).Length - 1;
            if (Enum.IsDefined(enumType, 0))
                flagPow--;

            long flagValue = 1;
            for (var i = 1; i <= flagPow; i++)
                flagValue |= (long)Math.Pow(2, i);

            return (Convert.ToInt64(thisEnum) & flagValue) == flagValue;
        }

        /// <summary>
        /// Проверяет наличие атрибута FlagsAttribute у перечисления
        /// </summary>
        /// <param name="enumType">Тип перечисления, которое требуется проверить</param>
        /// <exception cref="Exception">Обобщенное исключение возникает, если тип не содержит атрибут</exception>
        private static void CheckFlagsAttribute(Type enumType)
        {
            var enumAttr = enumType.GetCustomAttributes(typeof(FlagsAttribute), false);
            if (enumAttr.Length == 0)
                throw new Exception("Перечисление не содержит атрибут FlagsAttribute");
        }

        #endregion

        /// <summary>
        /// Получает атрибут указанного типа для значения перечисления
        /// </summary>
        /// <typeparam name="T">Тип атрибута</typeparam>
        /// <param name="enumVal">Значение перечисления</param>
        /// <returns>Атрибут</returns>
        public static T GetAttribute<T>(this Enum enumVal) where T : Attribute => GetAttributes<T>(enumVal).FirstOrDefault();
        
        /// <summary>
        /// Получает список атрибутов указанного типа для значения перечисления
        /// </summary>
        /// <typeparam name="T">Тип атрибута</typeparam>
        /// <param name="enumVal">Значение перечисления</param>
        /// <returns>Список атрибутов</returns>
        public static List<T> GetAttributes<T>(this Enum enumVal) where T : Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return attributes.Select(x => x as T).ToList();
        }
    }
}

