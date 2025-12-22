using System;
using System.Reflection;
// ReSharper disable StaticMemberInGenericType

namespace Promatis.Core
{
    /// <summary>
    /// Базовый класс для типов Singleton
    /// </summary>
    /// <typeparam name="T">Тип</typeparam>
    public abstract class SingletonBase<T> where T : class
    {
        private static volatile T _instance;
        private static readonly object Lock = new object();

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="SingletonBase{T}"/>
        /// </summary>
        protected SingletonBase()
        {
            var constructorPublic = typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            Guard.Against<Exception>(constructorPublic.Length > 0,
                $"Тип {typeof(T).FullName} содержит открытые конструкторы");

            var constructorNonPublic = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], null);
            Guard.Against<Exception>(constructorNonPublic == null || constructorNonPublic.IsAssembly,
                $"Тип {typeof(T).FullName} не содержит закрытых конструкторов");
        }

        /// <summary>
        /// Экземпляр типа
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Lock)
                        if (_instance == null)
                        {
                            _instance = Activator.CreateInstance<T>();
                        }
                }
                return _instance;
            }
        }
    }
}
