using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Promatis.Core.Extensions
{
    /// <summary>
    /// Методы расширения для типа <see cref="object"/>
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Данные о типе
        /// </summary>
        internal class TypeInfo
        {
            /// <summary>
            /// Значение по умелчанию
            /// </summary>
            internal object DefaultValue { get; }

            /// <summary>
            /// Метод конвертации из object
            /// </summary>
            internal MethodInfo ConvertMethod { get; set; }

            public TypeInfo(object defValue)
            {
                DefaultValue = defValue;
            }
        }

        private static Dictionary<Type, TypeInfo> _types;
        private static NumberFormatInfo _formatDot;
        private static NumberFormatInfo _formatComma;
        private static string[] _dateFormats;
        private static string[] _dateFormatsWithTime;

        private const NumberStyles NumberStyle = (NumberStyles.Float | NumberStyles.AllowThousands | NumberStyles.AllowExponent | NumberStyles.Number);

        /// <summary>
        /// Инициализирует коллекцию типов, и заполняет их методами конвертации из object
        /// </summary>
        private static void InitializeTypesCollection()
        {
            _types = new Dictionary<Type, TypeInfo>()
            {
                {typeof (bool), new TypeInfo(default(bool))},
                {typeof (char), new TypeInfo(default(char))},
                {typeof (byte), new TypeInfo(default(byte))},
                {typeof (sbyte), new TypeInfo(default(sbyte))},
                {typeof (short), new TypeInfo(default(short))},
                {typeof (ushort), new TypeInfo(default(ushort))},
                {typeof (int), new TypeInfo(default(int))},
                {typeof (uint), new TypeInfo(default(uint))},
                {typeof (long), new TypeInfo(default(long))},
                {typeof (ulong), new TypeInfo(default(ulong))}
            };

            foreach (var type in _types)
            {
                var convertMethod = GetConvertMethod(typeof(object), type.Key);
                type.Value.ConvertMethod = convertMethod ?? throw new Exception($"Can't find method Convert.To{type.Key.Name}");
            }
        }

        /// <summary>
        /// Инициализирует форматы представления данных
        /// </summary>
        private static void InitializeFormats()
        {
            _formatDot = new NumberFormatInfo()
            {
                NumberDecimalSeparator = ".",
                NumberGroupSeparator = "",
                NumberDecimalDigits = 12
            };

            _formatComma = new NumberFormatInfo()
            {
                NumberDecimalSeparator = ",",
                NumberGroupSeparator = "",
                NumberDecimalDigits = 12
            };

            _dateFormatsWithTime = new[]
            {
                "dd.MM.yyyy HH:mm:ss",
                "dd-MM-yyyy HH:mm:ss",
                "yyyy/MM/dd HH:mm:ss",
                "yyyy-MM-dd HH:mm:ss",
            };

            _dateFormats = new[]
            {
                "dd.MM.yyyy",
                "dd-MM-yyyy",
                "yyyy/MM/dd",
                "yyyy-MM-dd"
            };
        }

        static ObjectExtensions()
        {
            InitializeTypesCollection();
            InitializeFormats();
        }

        /// <summary>
        /// Получает метод конвертации изи типа <paramref name="from"/> в тип <paramref name="to"/>
        /// </summary>
        /// <param name="from">Исходный тип</param>
        /// <param name="to">Результирующий тип</param>
        /// <returns>Метод конвертации</returns>
        private static MethodInfo GetConvertMethod(Type from, Type to) => typeof(Convert).GetMethods().FirstOrDefault(item =>
            item.Name.Equals($"To{to.Name}", StringComparison.CurrentCultureIgnoreCase) &&
            item.GetParameters().Length.Equals(1) && item.GetParameters()[0].ParameterType == from);

        /// <summary>
        /// Получает значение указанного типа из текущего объекта
        /// </summary>
        /// <typeparam name="T">Тип возвращаемого значения</typeparam>
        /// <param name="data">Текущий объект</param>
        /// <returns></returns>
        public static T GetTypedValue<T>(this object data) => (T)GetTypedValue(data, typeof(T));

        /// <summary>
        /// Получает значение указанного типа из текущего объекта
        /// </summary>
        /// <param name="data">Текущий объект</param>
        /// <param name="type">Тип возвращаемого значения</param>
        /// <returns></returns>
        public static object GetTypedValue(this object data, Type type)
        {
            var empty = data == null;
            
            // Если тип объекта соответствует типу возвращаемого значение, то возвращаем текущий объект
            if (data != null && data.GetType() == type) return data;

            // Если тип объекта соответствует строковому типу, то возвращаем текущий объект или пустую строку
            if (type.IsStringType()) return data ?? string.Empty;

            // Если тип объекта содержится в коллекции типов, то возвращаем текущий объект
            if (_types.ContainsKey(type))
            {
                empty = empty || data is string && (string)data == "";
                return empty
                    ? _types[type].DefaultValue
                    : _types[type].ConvertMethod.Invoke(null, new[] { data });
            }

            if (type == typeof(double))
            {
                if (empty) return default(double);

                if (data.GetType().IsStringType()) return GetNumberFromString<double>((string)data);
            }
            else if (type == typeof(float))
            {
                if (empty) return default(float);

                if (data.GetType().IsStringType()) return GetNumberFromString<float>((string)data);
            }
            else if (type.IsDecimalType())
            {
                if (empty) return default(decimal);

                if (data.GetType().IsStringType()) return GetNumberFromString<decimal>((string)data);
            }
            else if (type == typeof(Guid))
            {
                if (empty) return Guid.Empty;

                if (data.GetType().IsStringType())
                {
                    var datastr = (string)data;
                    return datastr.Trim().Length > 0 ? new Guid(datastr) : Guid.Empty;
                }
            }
            else if (type == typeof(DateTime))
            {
                if (empty) return new DateTime(0);

                if (data.GetType().IsStringType())
                {
                    var datastr = (string)data;
                    if (!DateTime.TryParseExact(datastr, _dateFormatsWithTime, null, DateTimeStyles.None, out var val))
                        if (!DateTime.TryParseExact(datastr, _dateFormats, null, DateTimeStyles.None, out val))
                            throw new Exception($"Value is not {type}. Current value: \"{datastr}\"");

                    return DateTime.SpecifyKind(val, DateTimeKind.Local);
                }
            }
            else if (type.IsEnum)
            {
                if (empty) throw new Exception("data cannot be null");

                if (data.GetType().IsStringType()) return Enum.Parse(type, (string)data, true);
            }
            else if (type == typeof(byte[]))
            {
                if (empty) return null;

                if (data.GetType().IsStringType()) return Convert.FromBase64String((string)data);
            }

            if (empty) return null;

            var convertMethod = GetConvertMethod(data.GetType(), type);

            if (convertMethod == null)
                throw new NotImplementedException();

            return convertMethod.Invoke(null, new[] { data });
        }

        private static object GetNumberFromString<T>(string data)
        {
            var type = typeof(T);

            var methodInfo = type.GetMethods()
                .FirstOrDefault(
                    x =>
                        x.Name.Equals("TryParse", StringComparison.CurrentCultureIgnoreCase) &&
                        x.ReturnParameter != null
                        && x.ReturnParameter.ParameterType == typeof(bool)
                        && x.GetParameters().Length.Equals(4)
                        && x.GetParameters()[0].ParameterType == typeof(string)
                        && x.GetParameters()[1].ParameterType == typeof(NumberStyles)
                        && x.GetParameters()[2].ParameterType == typeof(IFormatProvider)
                        && x.GetParameters()[3].IsOut);

            if (methodInfo == null)
                throw new Exception($"Cannot find method \"TryParse\" from type {type}");

            var parameters = new object[] { data, NumberStyle, _formatComma, null };

            if (!(bool)methodInfo.Invoke(null, parameters))
            {
                parameters = new object[] { data, NumberStyle, _formatDot, null };

                if (!(bool)methodInfo.Invoke(null, parameters))
                    throw new Exception($"Value is not {type}. Current value: \"{data}\"");
            }

            return parameters[3];
        }

        /// <summary>
        /// Проверяет, содержится ли объект в перечне значений.
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "obj">Объект.</param>
        /// <param name = "values">Перечень значений.</param>
        /// <returns>Признак нахождения объекта</returns>
        public static bool EqualsAny<T>(this T obj, params T[] values) => Array.IndexOf(values, obj) != -1;

        /// <summary>
        /// Проверяет, отсутствует ли объект в перечне значений.
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "obj">Объект.</param>
        /// <param name = "values">Перечень значений.</param>
        /// <returns>Признак отсутствия объекта.</returns>
        public static bool EqualsNone<T>(this T obj, params T[] values) => !obj.EqualsAny(values);

        /// <summary>
        /// Выполняет метод заданный в параметре <paramref name="method"/> над текущим объектом
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="entity">Текущий объект</param>
        /// <param name="method">Метод</param>
        public static void Do<T>(this T entity, Action<T> method)
        {
            if (entity != null)
            {
                method(entity);
            }
        }

        #region SafeGet

        /// <summary>
        /// Возвращает значение типа <typeparamref name="TValue"/> полученое в результате выполнения метода <paramref name="method"></paramref> 
        /// если исходный объект не null, иначе значение заданное параметром <paramref name="defaultValue"/> 
        /// </summary>
        /// <typeparam name="TSource">Тип исходного объекта</typeparam>
        /// <typeparam name="TValue">Тип возвращаемого значения</typeparam>
        /// <param name="source">Исходный объект</param>
        /// <param name="method">Функция для получения требуемого объекта</param>
        /// <param name="defaultValue">Значение по умолчанию. Если не задано, то принимается равным значению по умолчанию для типа <typeparamref name="TValue"/></param>
        /// <returns></returns>
        public static TValue SafeGet<TSource, TValue>(this TSource source, Func<TSource, TValue> method, TValue defaultValue = default(TValue))
        {
            return source != null ? method(source) : defaultValue;
        }

        /// <summary>
        /// Возвращает результат выполнения метода, заданного в параметре <paramref name="method"></paramref> если исходный объект не null, 
        /// иначе результат выполнения метода, заданного в параметре <paramref name="defaultMethod"/> 
        /// </summary>
        /// <typeparam name="TSource">Тип исходного объекта</typeparam>
        /// <typeparam name="TValue">Тип возвращаемого значения</typeparam>
        /// <param name="source">Исходный объект</param>
        /// <param name="method">Функция для получения требуемого объекта</param>
        /// <param name="defaultMethod">Функция, выполняемая если исходный объект null</param>
        /// <returns></returns>
        public static TValue SafeGet<TSource, TValue>(this TSource source, Func<TSource, TValue> method, Func<TValue> defaultMethod)
        {
            return source != null ? method(source) : defaultMethod();
        }

        #endregion
    }
}
