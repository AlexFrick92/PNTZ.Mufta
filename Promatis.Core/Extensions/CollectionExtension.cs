using System;
using System.Collections.Generic;
using System.Linq;

namespace Promatis.Core.Extensions
{
    /// <summary>
    /// Методы расширения для коллекций
    /// </summary>
    public static class CollectiontExtension
    {
        /// <summary>
        /// Добавляет список элементов в коллекцию
        /// </summary>
        /// <typeparam name="T">Тип элементов коллекции</typeparam>
        /// <param name="list">Коллекция</param>
        /// <param name="items">Список элементов</param>
        /// <returns>Обновленная коллекция</returns>
        public static ICollection<T> AddRange<T>(this ICollection<T> list, IEnumerable<T> items)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (items == null) throw new ArgumentNullException(nameof(items));

            var itemsInternal = items.ToList(); // no side-effects
            if (itemsInternal.Count == 0)
                return list;

            foreach (var item in itemsInternal)
            {
                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// Добавляет список элементов в коллекцию
        /// </summary>
        /// <typeparam name="T">Тип элементов коллекции</typeparam>
        /// <param name="list">Коллекция</param>
        /// <param name="items">Список элементов</param>
        /// <returns>Обновленная коллекция</returns>
        public static ICollection<T> AddRange<T>(this ICollection<T> list, params T[] items)
        {
            return list.AddRange(items.AsEnumerable());
        }

        /// <summary>
        /// Удаляет элементы списка из коллекции
        /// </summary>
        /// <typeparam name="T">Тип элементов коллекции</typeparam>
        /// <param name="list">Коллекция</param>
        /// <param name="items">Список элементов</param>
        /// <returns>Обновленная коллекция</returns>
        public static ICollection<T> RemoveRange<T>(this ICollection<T> list, IEnumerable<T> items)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (items == null) throw new ArgumentNullException(nameof(items));

            var internalItems = items.ToList();

            if (internalItems.Count == 0)
                return list;

            foreach (var item in internalItems)
            {
                list.Remove(item);
            }

            return list;
        }

        /// <summary>
        /// Добавляет элемент в коллекцию, при его отсутствии в ней
        /// </summary>
        /// <typeparam name="TEntity">Тип элемента коллекции</typeparam>
        /// <typeparam name="TKey">Тип ключа сравнения</typeparam>
        /// <param name="list">Коллекция</param>
        /// <param name="item">Элемент</param>
        /// <param name="keySelector">Фукция сравнения по ключу</param>
        public static void AddIfNotExists<TEntity, TKey>(this ICollection<TEntity> list, TEntity item, Func<TEntity, TKey> keySelector)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            if (!list.Any(o => keySelector(o).Equals(keySelector(item))))
            {
                list.Add(item);
            }
        }

        /// <summary>
        /// Добавляет список элементов в коллекцию, если элементы списка отсутствуют в ней
        /// </summary>
        /// <typeparam name="TEntity">Тип элемента коллекции</typeparam>
        /// <typeparam name="TKey">Тип ключа сравнения</typeparam>
        /// <param name="list">Коллекция</param>
        /// <param name="items">Список элементов</param>
        /// <param name="keySelector">Фукция сравнения по ключу</param>
        public static void AddIfNotExists<TEntity, TKey>(this ICollection<TEntity> list, IEnumerable<TEntity> items, Func<TEntity, TKey> keySelector)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            foreach (var item in items)
            {
                if (!list.Any(o => keySelector(o).Equals(keySelector(item))))
                {
                    list.Add(item);
                }
            }
        }

        /// <summary>
        /// Проверяет что в коллекции есть любой из элементов списка
        /// </summary>
        /// <typeparam name="TEntity">Тип элемента коллекции</typeparam>
        /// <param name="list">Коллекция</param>
        /// <param name="entities">Список элементов</param>
        /// <returns>Если есть хоть один элемент - <c>True</c>, иначе - <c>False</c></returns>
        public static bool HasAny<TEntity>(this IEnumerable<TEntity> list, params TEntity[] entities)
        {
            return list != null && list.Any(entities.Contains);
        }

        /// <summary>
        /// Выполняет заданный метод для каждого элемента коллекции
        /// </summary>
        /// <typeparam name="T">Тип элемента коллекции</typeparam>
        /// <param name="items">Коллекция</param>
        /// <param name="method">Метод</param>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> method)
        {
            foreach (var item in items)
            {
                method(item);
            }
        }

        /// <summary>
        /// Получает хеш код коллекции
        /// </summary>
        /// <typeparam name="T">Тип элементов коллекции</typeparam>
        /// <param name="array">Коллекция</param>
        /// <returns></returns>
        public static int GetHashCode<T>(this IEnumerable<T> array)
        {
            return unchecked(array.Aggregate(17, (hash, item) => hash * 23 + item.GetHashCode()));
        }

        /// <summary>
        /// Возвращает массив последовательностей, разбитый на куски указанной размерности
        /// </summary>
        /// <typeparam name="T">Тид данных в исходной коллекции</typeparam>
        /// <param name="source">Исходная коллекция</param>
        /// <param name="length">Размерность, на которую нужно разбить коллекцию</param>
        /// <returns></returns>
        public static IEnumerable<T[]> Split<T>(this IEnumerable<T> source, int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            var section = new List<T>(length);
            foreach (var item in source)
            {
                section.Add(item);
                if (section.Count == length)
                {
                    yield return section.ToArray();
                    section.Clear();
                }
            }
            if (section.Count > 0)
                yield return section.ToArray();
        }

    }
}
