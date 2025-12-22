using System.IO;
using System.Linq;

namespace Promatis.Core.Helpers
{
    /// <summary>
    /// Вспомогательные мотодя для работы с файловой системой
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// Получает список наименований файлов библиотек из указанной папки
        /// </summary>
        /// <remarks>Фильтрация файлов идет по маске <c>*.dll ИЛИ *.exe</c></remarks>
        /// <param name="folderName">Путь к папке</param>
        /// <param name="includeSubdirectories">Признак включения содержимого вложенных папок. По умолчанию <c>true</c>, т.е. содержимое включено</param>
        /// <exception cref="DirectoryNotFoundException">Если указанная папка не существует</exception>
        /// <returns></returns>
        public static string[] GetAssemblyFilesFrom(string folderName, bool includeSubdirectories = true)
        {
            if (!Directory.Exists(folderName))
                throw new DirectoryNotFoundException(folderName);

            return Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories)
                .Where(x => x.EndsWith(".dll") || x.EndsWith(".exe")).ToArray();
        }
    }
}
