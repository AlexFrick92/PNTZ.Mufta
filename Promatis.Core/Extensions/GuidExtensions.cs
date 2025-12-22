using System;
using System.Linq;
using System.Text;

namespace Promatis.Core.Extensions
{
    /// <summary>
    /// Методы расширения для типа <see cref="Guid"/>
    /// </summary>
    public static class GuidExtensions
    {
        /// <summary>
        /// Признак того, что текущее значение равно <code>Guid.Empty</code>
        /// </summary>
        /// <param name="guid"></param>
        public static bool IsEmpty(this Guid guid) => guid == Guid.Empty;

        /// <summary>
        /// Признак того, что текущее значение не равно <code>Guid.Empty</code>
        /// </summary>
        /// <param name="guid"></param>
        public static bool IsNotEmpty(this Guid guid) => guid != Guid.Empty;

        /// <summary>
        /// Преобразует текущее знаение в массив байт, соотвествующий записи Guid в Oracle
        /// </summary>
        public static byte[] ToOracleRaw(this Guid guid)
        {
            var newBytes = new byte[16];
            var oldBytes = guid.ToByteArray();

            for (var i = 8; i < 16; i++)
                newBytes[i] = oldBytes[i];

            newBytes[3] = oldBytes[0];
            newBytes[2] = oldBytes[1];
            newBytes[1] = oldBytes[2];
            newBytes[0] = oldBytes[3];
            newBytes[5] = oldBytes[4];
            newBytes[4] = oldBytes[5];
            newBytes[6] = oldBytes[7];
            newBytes[7] = oldBytes[6];

            return newBytes;
        }

        /// <summary>
        /// Преобразует текущее значение в Guid
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>Если размерность массива равна 16, то Guid, иначе <c>Guid.Empty</c></returns>
        public static Guid ToGuid(this byte[] bytes)
        {
            if (bytes.Length != 16)
                return Guid.Empty;

            var newBytes = new byte[16];
            
            for (var i = 8; i < 16; i++)
                newBytes[i] = bytes[i];

            newBytes[3] = bytes[0];
            newBytes[2] = bytes[1];
            newBytes[1] = bytes[2];
            newBytes[0] = bytes[3];
            newBytes[5] = bytes[4];
            newBytes[4] = bytes[5];
            newBytes[6] = bytes[7];
            newBytes[7] = bytes[6];

            return new Guid(newBytes);
        }

        /// <summary>
        /// Возвращает строку в формате представления GUID
        /// </summary>
        /// <param name="bytes">Массив байт</param>
        /// <returns>Если размерность массива равна 16, то строка в формате Guid, иначе <c>string.Empty</c> </returns>
        public static string ToGuidFormatString(this byte[] bytes)
        {
            if (bytes.Length != 16)
                return string.Empty;
            var sb = new StringBuilder();
            var hyphenPositions = new[] {3, 5, 7, 9};
            for (var i = 0; i < bytes.Length; i++)
            {
                sb.Append(Convert.ToString(bytes[i], 16));
                if (hyphenPositions.Contains(i))
                    sb.Append("-");
            }

            return sb.ToString();
        }
    }
}
