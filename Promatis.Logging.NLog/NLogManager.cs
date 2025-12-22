using System;
using System.Configuration;
using System.IO;
using NLog;
using Promatis.Core.Extensions;

namespace Promatis.Logging.NLog
{
    /// <summary>
    /// Менеджер системы логирования NLog
    /// </summary>
    public static class NLogManager
    {
        private static bool _isInitialized;
        private static readonly object LockObject = new object();
        private const string ConfigFileName = "nlog.config";

        /// <summary>
        /// Инициализирует систему логирования
        /// </summary>
        private static void Initialize()
        {
            lock (LockObject)
            {
                if (_isInitialized)
                    return;
                var configFilePath = ConfigurationManager.AppSettings["logConfig"];
                if (configFilePath.IsEmpty())
                {
                    var configFile = new FileInfo(ConfigFileName);
                    if (!configFile.Exists)
                        configFilePath = Path.Combine(Path.Combine(Environment.CurrentDirectory, "config"),
                            ConfigFileName);
                }

                LogManager.LoadConfiguration(configFilePath);
                _isInitialized = true;
            }
        }

        /// <summary>
        /// Получает реализацию <see cref="ILogger"/> по имени
        /// </summary>
        /// <param name="name">Имя логера</param>
        /// <returns></returns>
        public static Core.Logging.ILogger GetLogger(string name)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            if(!_isInitialized)
                Initialize();
            return new NLogLogger(LogManager.GetLogger(name));
        }

        /// <summary>
        /// Конфигурирует систему на основе файла конфигурации
        /// </summary>
        /// <param name="configPath">Путь к файлу куонфигурации</param>
        public static void Configure(string configPath)
        {
            lock (LockObject)
            {
                _isInitialized = false;
                LogManager.LoadConfiguration(configPath);
                _isInitialized = true;
            }
        }

    }
}
