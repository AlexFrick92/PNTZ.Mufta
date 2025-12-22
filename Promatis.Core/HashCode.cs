using System.Linq;

namespace Promatis.Core
{
    /// <summary>
    /// Хеш код
    /// </summary>
    public struct HashCode
    {
        private readonly int _value;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="HashCode"/>
        /// </summary>
        /// <param name="value"></param>
        private HashCode(int value)
        {
            _value = value;
        }

        /// <summary>
        /// Оператор неявного приведения к Int
        /// </summary>
        /// <param name="hashCode"></param>
        /// <returns></returns>
        public static implicit operator int(HashCode hashCode)
        {
            return hashCode._value;
        }

        /// <summary>
        /// Хеш код объекта
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="item">Объект</param>
        /// <returns></returns>
        public static HashCode Of<T>(T item)
        {
            return new HashCode(GetHashCode(item));
        }

        /// <summary>
        /// Хеш код списка объектов
        /// </summary>
        /// <typeparam name="T">Тип объектов</typeparam>
        /// <param name="items">Список объектов</param>
        /// <returns></returns>
        public static HashCode Of<T>(params T[] items)
        {
            return new HashCode(17).AndEach(items);
        }

        /// <summary>
        /// Хеш код с учетом добавленного объекта
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="item">Добавленный объект</param>
        /// <returns></returns>
        public HashCode And<T>(T item)
        {
            return new HashCode(CombineHashCodes(_value, GetHashCode(item)));
        }

        /// <summary>
        /// Хеш код с учетом добавленного списка объектов
        /// </summary>
        /// <typeparam name="T">Тип объектов</typeparam>
        /// <param name="items">Список объектов</param>
        /// <returns></returns>
        public HashCode AndEach<T>(T[] items)
        {
            var hashCode = items.Any() ? items.Select(GetHashCode).Aggregate(CombineHashCodes) : 0;
            return new HashCode(CombineHashCodes(_value, hashCode));
        }

        private static int CombineHashCodes(int h1, int h2)
        {
            unchecked
            {
                // Key copied from System.Tuple so it must be the best way to combine hash codes or at least a good one.
                return ((h1 << 5) + h1) ^ h2;
            }
        }

        private static int GetHashCode<T>(T item)
        {
            return item == null ? 0 : item.GetHashCode();
        }
    }
}