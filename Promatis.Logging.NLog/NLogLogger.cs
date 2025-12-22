using Promatis.Core.Logging;
using System;
using INLogLogger = NLog.ILogger;

namespace Promatis.Logging.NLog
{
    /// <summary>
    /// Реализация <see cref="ILogger"/> для NLog
    /// </summary>
    internal class NLogLogger : ILogger
    {
        private readonly INLogLogger _logger;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="NLogLogger"/>
        /// </summary>
        /// <param name="logger"></param>
        public NLogLogger(INLogLogger logger) => _logger = logger;

        /// <inheritdoc />
        public ILogger GetLogger(string loggerName) => NLogManager.GetLogger(loggerName);

        /// <inheritdoc />
        public void Trace(string format, params object[] args) => _logger.Trace(format, args);

        /// <inheritdoc />
        public void Debug(string format, params object[] args) => _logger.Debug(format, args);

        /// <inheritdoc />
        public void Info(string format, params object[] args) => _logger.Info(format, args);

        /// <inheritdoc />
        public void Warn(string format, params object[] args) => _logger.Warn(format, args);

        /// <inheritdoc />
        public void Error(string format, params object[] args) => _logger.Error(format, args);

        /// <inheritdoc />
        public void Error(Exception exception, string message = null) => _logger.Error(exception, message);

        /// <inheritdoc />
        public void Error(Exception exception, string format, params object[] args) => _logger.Error(exception, format, args);

        public void Fatal(string format, params object[] args) => _logger.Fatal(format, args);

        /// <inheritdoc />
        public void Fatal(Exception exception, string message = null) => _logger.Fatal(exception, message);

        /// <inheritdoc />
        public void Fatal(Exception exception, string format, params object[] args) => _logger.Fatal(exception, format, args);
    }
}
