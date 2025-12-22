using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Promatis.Core.Enums
{
    /// <summary>
    /// Вспомогательный класс, описывающий методы работы с перечислениями
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Преобразовать перечислимый тип в словарь его значений
        /// </summary>
        /// <param name="t">Тип</param>
        /// <returns>Словарь значений</returns>
        public static Dictionary<int, string> EnumToDictionary(Type t)
        {
            if (!(t.IsSubclassOf(typeof (Enum))))
            {
                throw new InvalidOperationException(
                    $"Операция возможна только для типов производных от Enum, задан тип {t}");
            }
            return Enum.GetValues(t).Cast<object>().ToDictionary(v => (int) v, v => Enum.GetName(t, (int) v));
        }

        /// <summary>
        /// Получает описание значений перечисления по типу, в формате (значение, описание)
        /// </summary>
        /// <typeparam name="T">Тип перечисления </typeparam>
        /// <returns>Словарь описаний</returns>
        public static Dictionary<string,string> GetDescribedEnum<T>()
        {
            var result = new Dictionary<string, string>();
            Type type = typeof (T);
            if (type.IsEnum)
            {
                foreach (var value in Enum.GetValues(type))
                {
                    var field = type.GetField(value.ToString());
                    result.Add(value.ToString(), Attribute.GetCustomAttribute(field,
                        typeof(DescriptionAttribute)) is DescriptionAttribute attribute ? attribute.Description : value.ToString());
                }
            }
            return result;
        }

        /// <summary>
        /// Получает описание значений перечисления по типу, в формате (значение, описание)
        /// </summary>
        /// <typeparam name="T">Тип перечисления </typeparam>
        /// <returns>Словарь описаний</returns>
        public static List<KeyValuePair<T, string>> GetDescribedCollection<T>()
        {
            var result = new List<KeyValuePair<T, string>>();
            var type = typeof (T);
            if (type.IsEnum)
            {
                result.AddRange(from object value in Enum.GetValues(type)
                    let field = type.GetField(value.ToString())
                    let attribute =
                        Attribute.GetCustomAttribute(field, typeof (DescriptionAttribute)) as DescriptionAttribute
                    select
                        new KeyValuePair<T, string>((T) value,
                            attribute != null ? attribute.Description : value.ToString()));
            }
            return result;
        }

        /// <summary>
        /// Получает значение перечисления по его описанию
        /// </summary>
        /// <typeparam name="T">Тип перечисления</typeparam>
        /// <returns>Значение перечисления</returns>
        public static T GetValueByDescription<T>(string description)
        {
            if (typeof(T).IsEnum)
            {
                foreach (var value in from object value in Enum.GetValues(typeof(T)) let field = typeof(T).GetField(value.ToString()) let attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute where attribute != null && attribute.Description.Equals(description) select value)
                {
                    return (T)value;
                }
            }
            return default(T);
        }
    }
}