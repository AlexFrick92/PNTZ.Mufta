using System;
using System.Linq;
using System.Threading;

namespace Promatis.Core.Threading
{
    /// <summary>
    /// Вспомогательный класс для блокирования доступа между потоками
    /// </summary>
    public class WaitLock : IDisposable
    {
        private readonly object[] _padlocks;
        private readonly bool[] _securedFlags;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="WaitLock"/>
        /// </summary>
        /// <param name="milliSecondTimeout">Период времени в миллисекундах, в течении которого будут попытки получить монопольную блокировку объекта</param>
        /// <param name="padlock">Блокируемый объекта</param>
        public WaitLock(int milliSecondTimeout, object padlock)
        {
            _padlocks = new[] { padlock };
            _securedFlags = new[] { Monitor.TryEnter(padlock, milliSecondTimeout) };
        }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="WaitLock"/>
        /// </summary>
        /// <param name="milliSecondTimeout">Период времени в миллисекундах, в течении которого будут попытки получить монопольную блокировку каждого объекта в списке</param>
        /// <param name="padlocks">Список блокируемых объектов</param>
        public WaitLock(int milliSecondTimeout, params object[] padlocks)
        {
            _padlocks = padlocks;
            _securedFlags = new bool[_padlocks.Length];
            for (int i = 0; i < _padlocks.Length; i++)
                _securedFlags[i] = Monitor.TryEnter(padlocks[i], milliSecondTimeout);
        }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="WaitLock"/>
        /// </summary>
        /// <remarks>Попутки получить монопольную блокировку будут в течении 1 сек</remarks>
        /// <param name="padlocks">Список блокируемых объектов</param>
        public WaitLock(params object[] padlocks)
        {
            _padlocks = padlocks;
            _securedFlags = new bool[_padlocks.Length];
            for (var i = 0; i < _padlocks.Length; i++)
                _securedFlags[i] = Monitor.TryEnter(padlocks[i], 1000);
        }

        /// <summary>
        /// Признак того что блокировка получена
        /// </summary>
        public bool IsLocked => _securedFlags.All(s => s);

        /// <summary>
        /// Блокирует список объектов для выполнения кода 
        /// </summary>
        /// <param name="padlocks">Список объектов</param>
        /// <param name="millisecondTimeout">Период времени в миллисекундах, в течении которого будут попытки получить монопольную блокировку каждого объекта в списке</param>
        /// <param name="codeToRun">Код, выполняемый во время блокировки</param>
        /// <exception cref="TimeoutException">Возникает если в течении заданного периода блокировка не была получена</exception>
        public static void Lock(object[] padlocks, int millisecondTimeout, Action codeToRun)
        {
            using (var bolt = new WaitLock(millisecondTimeout, padlocks))
                if (bolt.IsLocked)
                    codeToRun();
                else
                    throw new TimeoutException($"Safe.Lock wasn't able to acquire a lock in {millisecondTimeout}ms");
        }

        /// <summary>
        /// Блокирует объект для выполнения кода 
        /// </summary>
        /// <param name="padlock">J,]trn</param>
        /// <param name="millisecondTimeout">Период времени в миллисекундах, в течении которого будут попытки получить монопольную блокировку объекта</param>
        /// <param name="codeToRun">Код, выполняемый во время блокировки</param>
        /// <exception cref="TimeoutException">Возникает если в течении заданного периода блокировка не была получена</exception>
        public static void Lock(object padlock, int millisecondTimeout, Action codeToRun)
        {
            using (var bolt = new WaitLock(millisecondTimeout, padlock))
                if (bolt.IsLocked)
                    codeToRun();
                else
                    throw new TimeoutException($"Safe.Lock wasn't able to acquire a lock in {millisecondTimeout}ms");
        }

        #region Implementation of IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            for (var i = 0; i < _securedFlags.Length; i++)
                if (_securedFlags[i])
                {
                    Monitor.Exit(_padlocks[i]);
                    _securedFlags[i] = false;
                }
        }

        #endregion
    }
}
