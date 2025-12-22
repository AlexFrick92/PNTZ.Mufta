using System;
using System.Threading;
using Promatis.Core.Events;

namespace Promatis.Core.Extensions
{
    /// <summary>
    /// Методы расширений для событий
    /// </summary>
    public static class EventExtensions
    {
        /// <summary>
        /// Инициирует событие
        /// </summary>
        /// <param name="event">Событие</param>
        /// <param name="sender">Источник события</param>
        public static void Raise(this EventHandler @event, object sender) =>
            Volatile.Read(ref @event)?.Invoke(sender, EventArgs.Empty);

        /// <summary>
        /// Инициирует событие c передачей данных
        /// </summary>
        /// <typeparam name="TEventArgs">Тип объекта наследующего от EventArgs</typeparam>
        /// <param name="event">Событие</param>
        /// <param name="sender">Источник события</param>
        /// <param name="args">Объект типа EventArgs содержащий данные события</param>
        public static void Raise<TEventArgs>(this EventHandler<TEventArgs> @event, object sender, TEventArgs args)
            where TEventArgs : EventArgs => Volatile.Read(ref @event)?.Invoke(sender, args);

        /// <summary>
        /// Инициирует событие c передачей данных
        /// </summary>
        /// <typeparam name="T">Тип данных для события</typeparam>
        /// <param name="event">Событие</param>
        /// <param name="sender">Источник события</param>
        /// <param name="args">Данные для события</param>
        public static void Raise<T>(this EventHandler<EventArgs<T>> @event, object sender, T args) =>
            Volatile.Read(ref @event)?.Invoke(sender, new EventArgs<T>(args));
    }
}