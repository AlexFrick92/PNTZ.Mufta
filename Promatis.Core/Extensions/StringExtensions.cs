using Promatis.Core.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Promatis.Core.Extensions
{
    /// <summary>
    /// Методы расширений для строк
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Форматирует строку с параметрами, используя string.Format.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Args(this String target, params object[] args) => string.Format(target, args);

        #region IsEmpty, IsNotEmpty

        /// <summary>
        /// Указывает, является ли заданная строка значением null, пустой строкой или строкой, состоящей только из пробельных символов.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string value) => string.IsNullOrWhiteSpace(value);

        /// <summary>
        /// Указывает что заданная строка НЕ является значением null, пустой строкой или строкой, состоящей только из пробельных символов.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNotEmpty(this string value) => !value.IsEmpty();

        #endregion

        #region NotNull, NotEmpty

        /// <summary>
        /// Возвращает пустую строку если заданная строка является значением null, в противном случае саму строку
        /// </summary>
        /// <param name="target">Строка</param>
        /// <returns></returns>
        public static string NotNull(this string target) => target ?? string.Empty;

        /// <summary>
        ///  Возвращает значение по умолчанию если заданная строка пустая, в противном случае саму строку
        /// </summary>
        /// <param name = "value">The string to check.</param>
        /// <param name = "defaultValue">The default value.</param>
        /// <returns>Either the string or the default value.</returns>
        public static string NotEmpty(this string value, string defaultValue) => value.IsNotEmpty() ? value : defaultValue;

        #endregion

        #region ContainsAny, ContainsAll

        /// <summary>
        /// Проверяет вхождение любой из указаных строк в данном экземпляре
        /// </summary>
        /// <param name="inputValue"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool ContainsAny(this string inputValue, params string[] values)
        {
            return inputValue.IsNotEmpty() && values.Any(p => inputValue.IndexOf(p, StringComparison.Ordinal) != -1);
        }

        /// <summary>
        /// Проверяет вхождение всех указаных строк в данном экземпляре
        /// </summary>
        /// <param name="inputValue"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool ContainsAll(this string inputValue, params string[] values)
        {
            return inputValue.IsNotEmpty() && values.All(p => inputValue.IndexOf(p, StringComparison.Ordinal) != -1);
        }

        #endregion

        #region LikeAny, Like

        /// <summary>
        /// Проверяет соответствие строки любому из указанных шаблонов
        /// </summary>
        /// <param name="value">Строка<see cref="System.String"/> object</param>
        /// <param name="patterns">Шаблоны</param>
        /// <returns>Признак соответствия</returns>
        public static bool LikeAny(this string value, params string[] patterns) => patterns.Any(value.Like);

        /// <summary>
        /// Проверяет соответствие строки указанному шаблону
        /// </summary>
        /// <param name="value">Строка</param>
        /// <param name="pattern">Шаблон</param>
        /// <returns>Признак соответствия</returns>
        public static bool Like(this string value, string pattern)
        {
            if (value == pattern || pattern == "*") return true;

            if (pattern.StartsWith("*"))
            {
                return value.Where((t, index) => value.Substring(index).Like(pattern.Substring(1))).Any();
            }
            if (pattern[0] == value[0])
            {
                return value.Substring(1).Like(pattern.Substring(1));
            }
            return false;
        }

        #endregion

        /// <summary>
        /// Возвращает заданную строку, если исходная строка пустая
        /// </summary>
        /// <param name="source">Исходная строка</param>
        /// <param name="given">Заданная строка</param>
        /// <returns></returns>
        public static string Or(this String source, string given) => source.IsEmpty() ? given : source;

        /// <summary>
        /// Разбивает строку на несколько строк с длиной не превышающей заданную
        /// </summary>
        /// <remarks>Строка разбивается по пробелам, либо по символам окончания строки</remarks>
        /// <param name="source">Исходный текст</param>
        /// <param name="width">Максимальная длина строки</param>
        /// <returns>Коллекция строк</returns>
        public static IList<string> Wrap(this String source, int width)
        {
            var format = $@"(.{{1,{width}}})(\s+|$\n?)";
            var matches = Regex.Matches(source, format);
            var result = new List<string>(matches.Count);
            result.AddRange(
                from object match in matches
                select match.ToString());
            return result;
        }

        /// <summary>
        /// Заменяет разделитель целой и дробной части в исходной строке на заданный для текущей культуры
        /// </summary>
        /// <param name="source">Исходная строка</param>
        /// <returns></returns>
        public static string IntegerToCurrentCulture(this string source)
        {
            return source ??
                source.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                .Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        }

        private const string _key = "kIx8Z9ZGL37lQKmgnJZs6+wP6xFRfIm9hI6J+i0qRwo=";
        
        /// <summary>
        /// Зашифровывает исходный текст
        /// </summary>
        /// <param name="source">Исходный текст</param>
        /// <returns>Строка, предствляющая собой зашифрованый исходный текст в формате пригдном для использования <see cref="Decode(string)"/></returns>
        public static string Encode(this string source)
        {
            var coder = new AesManaged { Key = Convert.FromBase64String(_key) };
            var encriptor = coder.CreateEncryptor();
            byte[] data = Encoding.UTF8.GetBytes(source);
            byte[] decripted = encriptor.TransformFinalBlock(data, 0, data.Length);
            return $"{Convert.ToBase64String(decripted)}:{Convert.ToBase64String(coder.IV)}";
        }

        /// <summary>
        /// Расшифровывает исходный текст
        /// </summary>
        /// <param name="source">Исходный текст</param>
        /// <returns>Исходный текст, который был зашифрован в текущей строке при помощи <see cref="Encode(string)"/></returns>
        public static string Decode(this string source)
        {
            string[] data = source.Split(':');
            if (data.Length != 2)
                throw new FormatException(Localization.StringExtensions_WrongEncryptedData);

            var decoder = new AesManaged { Key = Convert.FromBase64String(_key), IV = Convert.FromBase64String(data[1]) };
            var decryptor = decoder.CreateDecryptor();
            byte[] enctypted = Convert.FromBase64String(data[0]);
            byte[] decrypted = decryptor.TransformFinalBlock(enctypted, 0, enctypted.Length);
            return Encoding.UTF8.GetString(decrypted);
        }


        /// <summary>
        /// Получить цвет из строки через MD5 хеш
        /// </summary>
        public static Color ToColorHash(this string input) => ToColorHash(input, 0.24, 0.8, 0.46, 0.88);
        /// <summary>
        /// Получить цвет из строки через MD5 хеш
        /// </summary>                
        public static Color ToColorHash(this string input, 
            double saturationMin,
            double saturationMax,
            double lightnessMin,
            double lightnessMax)
        {
            if (string.IsNullOrEmpty(input)) input = string.Empty;

            // Используем MD5 для качественного хеширования
            using (var md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Используем разные части хеша для Hue, Saturation и Lightness
                uint hash1 = BitConverter.ToUInt32(hashBytes, 0);
                uint hash2 = BitConverter.ToUInt32(hashBytes, 4);
                uint hash3 = BitConverter.ToUInt32(hashBytes, 8);

                // Hue: 0-360 градусов
                double hue = (hash1 % 360);

                // Saturation: варьируем в диапазоне [Min, Max]
                double saturationRange = saturationMax - saturationMin;
                double saturation = saturationMin + (hash2 % 1000) / 1000.0 * saturationRange;

                // Lightness: варьируем в диапазоне [Min, Max]
                double lightnessRange = lightnessMax - lightnessMin;
                double lightness = lightnessMin + (hash3 % 1000) / 1000.0 * lightnessRange;

                return HslToRgb(hue, saturation, lightness);                
            }
        }
        /// <summary>
        /// Стандартное преобразование HSL в RGB
        /// Выглядит странно, но работает правильно
        /// </summary>        
        /// <returns></returns>
        private static Color HslToRgb(double h, double s, double l)
        {
            double c = (1 - Math.Abs(2 * l - 1)) * s;
            double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            double m = l - c / 2;

            double r, g, b;
            int sector = (int)(h / 60);

            switch (sector)
            {
                case 0: r = c; g = x; b = 0; break;
                case 1: r = x; g = c; b = 0; break;
                case 2: r = 0; g = c; b = x; break;
                case 3: r = 0; g = x; b = c; break;
                case 4: r = x; g = 0; b = c; break;
                default: r = c; g = 0; b = x; break;
            }

            return Color.FromArgb(
                (byte)((r + m) * 255),
                (byte)((g + m) * 255),
                (byte)((b + m) * 255)
            );
        }
    }
}