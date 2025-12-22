using System;

namespace Promatis.Core.Logging
{
	/// <summary>
	/// Вспомогательный класс, реализующий систему логирования
	/// </summary>
	public static class Logger
	{
		private static volatile ILogger _instance;
	    private static readonly object Sync = new object();

        /// <summary>
        /// Экземпляр реализации системы
        /// </summary>
        public static ILogger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Sync)
                        if (_instance == null)
                        {
                            _instance = IoC.Resolve<ILogger>() ?? new ConsoleLogger();
                        }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Получает именнованный логгер 
        /// </summary>
        public static ILogger GetNamedLogger(string loggerName) => Instance.GetLogger(loggerName);

        /// <summary> 
        /// Записывает в лог форматированную строку с уровнем <c>TRACE</c>
        /// </summary>
        /// <param name="format">Формат строки</param>
        /// <param name="args">Аргументы</param>
        public static void Trace(string format, params object[] args) => Instance.Trace(format, args);

        /// <summary> 
        /// Записывает в лог форматированную строку с уровнем <c>DEBUG</c>
        /// </summary>
        /// <param name="format">Формат строки</param>
        /// <param name="args">Аргументы</param>
        public static void Debug(string format, params object[] args) => Instance.Debug(format, args);

        /// <summary> 
        /// Записывает в лог форматированную строку с уровнем <c>ERROR</c>
        /// </summary>
        /// <param name="format">Формат строки</param>
        /// <param name="args">Аргументы</param>
        public static void Error(string format, params object[] args) => Instance.Error(format, args);

        /// <summary>
        /// Записывает в лог данные исключения и дополнительное сообщение с уровнем <c>ERROR</c>
        /// </summary>
        /// <param name="exception">Исключение</param>
        /// <param name="message">Сообщение</param>
        public static void Error(Exception exception, string message = null) => Instance.Error(exception, message);

        /// <summary>
        /// Записывает в лог данные исключения и дополнительное форматированное сообщение с уровнем <c>ERROR</c>
        /// </summary>
        /// <param name="exception">Исключение</param>
        /// <param name="format">Формат строки</param>
        /// <param name="args">Фаргументы</param>
        public static void Error(Exception exception, string format, params object[] args) => Instance.Error(exception, format, args);

        /// <summary> 
        /// Записывает в лог форматированную строку с уровнем <c>FATAL</c>
        /// </summary>
        /// <param name="format">Формат строки</param>
        /// <param name="args">Аргументы</param>
        public static void Fatal(string format, params object[] args) => Instance.Fatal(format, args);

        /// <summary>
        /// Записывает в лог данные исключения и дополнительное сообщение с уровнем <c>FATAL</c>
        /// </summary>
        /// <param name="exception">Исключение</param>
        /// <param name="message">Сообщение</param>
        public static void Fatal(Exception exception, string message = null) => Instance.Fatal(exception, message);

        /// <summary>
        /// Записывает в лог данные исключения и дополнительное форматированное сообщение с уровнем <c>FATAL</c>
        /// </summary>
        /// <param name="exception">Исключение</param>
        /// <param name="format">Формат строки</param>
        /// <param name="args">Фаргументы</param>
        public static void Fatal(Exception exception, string format, params object[] args) => Instance.Fatal(exception, format, args);

        /// <summary> 
        /// Записывает в лог форматированную строку с уровнем <c>INFO</c>
        /// </summary>
        /// <param name="format">Формат строки</param>
        /// <param name="args">Аргументы</param>
        public static void Info(string format, params object[] args) => Instance.Info(format, args);

        /// <summary> 
        /// Записывает в лог форматированную строку с уровнем <c>WARN</c>
        /// </summary>
        /// <param name="format">Формат строки</param>
        /// <param name="args">Аргументы</param>
        public static void Warn(string format, params object[] args) => Instance.Warn(format, args);
    }
}
