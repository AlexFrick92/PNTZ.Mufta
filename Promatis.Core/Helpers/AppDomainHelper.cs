using System;
using Promatis.Core.Logging;
using Promatis.Core.Resources;

namespace Promatis.Core.Helpers
{
    /// <summary>
    /// Вспомогтельный класс для работы с доменами приложений
    /// </summary>
    public class AppDomainHelper
    {
        /// <summary>
        /// Выгружает домен приложения
        /// </summary>
        /// <param name="appDomain">Выгружаемый домен</param>
        /// <param name="logger">Логгер</param>
        public static void UnloadAppDomain(AppDomain appDomain, ILogger logger)
        {
            string domainName = appDomain.FriendlyName;
            try
            {
                AppDomain.Unload(appDomain);
                logger.Debug(Localization.AppDomainHelper_AppDomainUnloaded, domainName);
            }
            catch (CannotUnloadAppDomainException ex)
            {
                logger.Error(ex, Localization.AppDomainHelper_AppDomainUnableUnload, appDomain.FriendlyName);

                // Попробуем выгрузить его за несколько попыток с интервалами ожидания 3 секунды
                for (int i = 0; i <= 10; i++)
                {
                    System.Threading.Thread.Sleep(3000);
                    try
                    {
                        AppDomain.Unload(appDomain);
                        break;
                    }
                    catch (CannotUnloadAppDomainException)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, Localization.AppDomainHelper_AppDomainUnloadError, domainName);
            }
        }
    }
}
