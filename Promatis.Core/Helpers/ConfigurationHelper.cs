using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace Promatis.Core.Helpers
{
    /// <summary>
    /// Вспомогательнфй класс для работы с конфигурацией приложения
    /// </summary>
    public static class ConfigurationHelper
    {
        ///// <summary>
        ///// Получает список наименований файлов библиотек из указанной папки
        ///// </summary>
        ///// <remarks>Фильтрация файлов идет по маске <c>*.dll ИЛИ *.exe</c></remarks>
        ///// <param name="folderName">Путь к папке</param>
        ///// <param name="includeSubdirectories">Признак включения содержимого вложенных папок. По умолчанию <c>true</c>, т.е. содержимое включено</param>
        ///// <exception cref="DirectoryNotFoundException">Если указанная папка не существует</exception>
        ///// <returns></returns>
        //public static Tuple<string, string, string>[] GetRedirectedAssemblies()
        //{
        //    var result = new List<Tuple<string, string, string>>();
        //    var cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        //    var bindingSection = cfg.GetSection("assemblyBinding");
        //    if (bindingSection == null)
        //        return result.ToArray();
        //    bindingSection

        //}
    }
}
