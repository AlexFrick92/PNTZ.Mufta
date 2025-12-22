using System;

namespace Promatis.Core.Logging
{
	/// <summary> 
	/// Интерфейс логирования 
	/// </summary>
	public interface ILogger
	{
        /// <summary> 
        /// Получает логер по имени
        /// </summary>
	    ILogger GetLogger(string loggerName);

        /// <summary> 
        /// Записывает в лог форматированную строку с уровнем <c>TRACE</c>
        /// </summary>
        /// <param name="format">Формат строки</param>
        /// <param name="args">Аргументы</param>
        void Trace(string format, params object[] args);

        /// <summary> 
        /// Записывает в лог форматированную строку с уровнем <c>DEBUG</c>
        /// </summary>
        /// <param name="format">Формат строки</param>
        /// <param name="args">Аргументы</param>
        void Debug(string format, params object[] args);

	    /// <summary> 
	    /// Записывает в лог форматированную строку с уровнем <c>INFO</c>
	    /// </summary>
	    /// <param name="format">Формат строки</param>
	    /// <param name="args">Аргументы</param>
		void Info(string format, params object[] args);

	    /// <summary> 
	    /// Записывает в лог форматированную строку с уровнем <c>WARN</c>
	    /// </summary>
	    /// <param name="format">Формат строки</param>
	    /// <param name="args">Аргументы</param>
		void Warn(string format, params object[] args);

	    /// <summary> 
	    /// Записывает в лог форматированную строку с уровнем <c>ERROR</c>
	    /// </summary>
	    /// <param name="format">Формат строки</param>
	    /// <param name="args">Аргументы</param>
		void Error(string format, params object[] args);

	    /// <summary>
	    /// Записывает в лог данные исключения и дополнительное сообщение с уровнем <c>ERROR</c>
	    /// </summary>
	    /// <param name="exception">Исключение</param>
	    /// <param name="message">Сообщение</param>
		void Error(Exception exception, string message = null);

	    /// <summary>
	    /// Записывает в лог данные исключения и дополнительное форматированное сообщение с уровнем <c>ERROR</c>
	    /// </summary>
	    /// <param name="exception">Исключение</param>
	    /// <param name="format">Формат строки</param>
	    /// <param name="args">Фаргументы</param>
		void Error(Exception exception, string format, params object[] args);

	    /// <summary> 
	    /// Записывает в лог форматированную строку с уровнем <c>FATAL</c>
	    /// </summary>
	    /// <param name="format">Формат строки</param>
	    /// <param name="args">Аргументы</param>
		void Fatal(string format, params object[] args);

	    /// <summary>
	    /// Записывает в лог данные исключения и дополнительное сообщение с уровнем <c>FATAL</c>
	    /// </summary>
	    /// <param name="exception">Исключение</param>
	    /// <param name="message">Сообщение</param>
		void Fatal(Exception exception, string message = null);

	    /// <summary>
	    /// Записывает в лог данные исключения и дополнительное форматированное сообщение с уровнем <c>FATAL</c>
	    /// </summary>
	    /// <param name="exception">Исключение</param>
	    /// <param name="format">Формат строки</param>
	    /// <param name="args">Фаргументы</param>
		void Fatal(Exception exception, string format, params object[] args);

    }
}
