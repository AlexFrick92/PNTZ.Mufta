using System;

namespace Promatis.Core.Events
{
    /// <summary>
    /// Аргумент содержащий в себе данные указанного типа
    /// </summary>
    /// <typeparam name="T">Тип данных</typeparam>
    public class EventArgs<T> : EventArgs
    {
        /// <summary>
        /// Данные
        /// </summary>
        public T Data { get; }
        
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="EventArgs{T}"/>
        /// </summary>
        /// <param name="data"></param>
        public EventArgs(T data)
        {
            Data = data;
        }
    }
}
