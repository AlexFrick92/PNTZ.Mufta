using System;

namespace Promatis.Core.Logging
{
    /// <summary>
    /// Консольный логгер
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        /// <inheritdoc />
        public ILogger GetLogger(string loggerName) => new ConsoleLogger();

        /// <inheritdoc />
        public void Trace(string format, params object[] args)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(format, args);
            Console.ForegroundColor = color;
        }

        /// <inheritdoc />
        public void Debug(string format, params object[] args) => Console.WriteLine(format, args);

        /// <inheritdoc />
        public void Error(string format, params object[] args)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(format, args);
            Console.ForegroundColor = color;
        }

        /// <inheritdoc />
        public void Error(Exception exception, string message = null)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            WriteExceptionLine(exception);
            Console.WriteLine(exception.StackTrace);
            Console.ForegroundColor = color;
        }

        /// <inheritdoc />
        public void Error(Exception exception, string format, params object[] args)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(format, args);
            WriteExceptionLine(exception);
            Console.WriteLine(exception.StackTrace);
            Console.ForegroundColor = color;
        }

        /// <inheritdoc />
        public void Fatal(string format, params object[] args)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(format, args);
            Console.ForegroundColor = color;
        }

        /// <inheritdoc />
        public void Fatal(Exception exception, string message = null)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            WriteExceptionLine(exception);
            Console.WriteLine(exception.StackTrace);
            Console.ForegroundColor = color;
        }

        /// <inheritdoc />
        public void Fatal(Exception exception, string format, params object[] args)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(format, args);
            Console.WriteLine(exception.Message);
            WriteExceptionLine(exception);
            Console.WriteLine(exception.StackTrace);
            Console.ForegroundColor = color;
        }

        /// <inheritdoc />
        public void Info(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }

        /// <inheritdoc />
        public void Warn(string format, params object[] args)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(format, args);
            Console.ForegroundColor = color;
        }

        private void WriteExceptionLine(Exception ex)
        {
            if (ex != null)
            {
                Console.WriteLine(ex.Message);
                WriteExceptionLine(ex.InnerException);
            }
        }
    }
}
