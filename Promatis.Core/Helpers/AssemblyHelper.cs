using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Promatis.Core.Extensions;
using Promatis.Core.Resources;

namespace Promatis.Core.Helpers
{
    /// <summary>
    /// Вспомогательный класс, содержащий в себе статические методы для работы со сборками
    /// </summary>
    public class AssemblyHelper
    {
        /// <summary>
        /// Загружает сборки, имена которых соответствуют шаблону, указанному в <paramref name="searchPattern"/>  
        /// </summary>
        /// <para>Поиск ведется среди файлов с расширением <c>DLL</c>. Если путь не задан, поиск осуществляется в текущем каталоге</para>
        /// <param name="searchPattern">Маска наименования.</param> 
        /// <param name="path">Путь, по которому находятся требуемые сборки</param>
        /// <param name="includeExe">Признак, указывающий на то что в поиске участвуют файлы с расширением <c>exe</c></param>
        /// <exception cref="FileLoadException"></exception>
        /// <returns>Коллекция <see cref="Assembly"/>.</returns>
        public static IEnumerable<Assembly> LoadAssemblies(string searchPattern = "", string path = null, bool includeExe = false)
        {
            var assemblies = new List<Assembly>();
            
            var directoryInfo = new DirectoryInfo(path ?? Environment.CurrentDirectory);
            if (!directoryInfo.Exists)
                return assemblies;
            assemblies = directoryInfo.GetFiles($"*{searchPattern}*.dll").Select(file => LoadAssembly(file.FullName)).ToList();
            if (includeExe)
                assemblies.AddRange(directoryInfo.GetFiles($"*{searchPattern}*.exe")
                    .Select(file => LoadAssembly(file.FullName)));
            return assemblies;
        }

        /// <summary>
        /// Загружает сборки, имена которых указаны в <paramref name="assemblyNames"/>
        /// </summary>
        /// <param name="assemblyNames">Коллекция имен сборок.</param>
        /// <exception cref="FileLoadException"></exception>
        /// <returns>Коллекция <see cref="Assembly"/></returns>
        public static IEnumerable<Assembly> LoadAssemblies(IEnumerable<string> assemblyNames) => assemblyNames.Select(LoadAssembly);

        /// <summary>
        /// Загружает сборку по имени.
        /// </summary>
        /// <param name="assemblyName">Имя сборки.</param>
        /// <exception cref="FileLoadException"></exception>
        /// <returns>Экземпляр <see cref="Assembly"/></returns>
        public static Assembly LoadAssembly(string assemblyName)
        {
            try
            {
                return Assembly.LoadFrom(assemblyName);
            }
            catch (Exception ex)
            {
                throw new FileLoadException(Localization.AssemblyHelper_AssemblyNotLoaded.Args(assemblyName), ex);
            }
        }

        /// <summary>
        /// Загружает сборку по её идентификатору.
        /// </summary>
        /// <param name="assemblyName">Имя сборки.</param>
        /// <exception cref="FileLoadException"></exception>
        /// <returns>Экземпляр <see cref="Assembly"/></returns>
        public static Assembly LoadAssembly(AssemblyName assemblyName)
        {
            try
            {
                return Assembly.Load(assemblyName);
            }
            catch (Exception ex)
            {
                throw new FileLoadException(Localization.AssemblyHelper_AssemblyNotLoaded.Args(assemblyName.FullName), ex);
            }
        }

        /// <summary>
        /// Загружает сборку по её идентификатору.
        /// <para>Безопасная загрузка. Все исключения подавляются.</para>
        /// </summary>
        /// <param name="assemblyName">Имя сборки.</param>
        /// <returns>Экземпляр <see cref="Assembly"/></returns>
        public static Assembly LoadAssemblySafe(AssemblyName assemblyName)
        {
            try
            {
                return Assembly.Load(assemblyName);
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// Перенаправляет загрузку сборки со строгим именем на идентичную по наименованию сборку с указанными параметрами
        /// </summary>
        /// <param name="shortName">Наименование сборки</param>
        /// <param name="targetVersion">Версия</param>
        /// <param name="publicKeyToken">Публичный ключ</param>
        public static void RedirectAssembly(string shortName, Version targetVersion, string publicKeyToken)
        {
            Assembly ResolveAssemblyHandler(object sender, ResolveEventArgs args)
            {
                var requestedAssembly = new AssemblyName(args.Name);
                if (requestedAssembly.Name != shortName) return null;

                requestedAssembly.Version = targetVersion;
                requestedAssembly.SetPublicKeyToken(new AssemblyName("x, PublicKeyToken=" + publicKeyToken).GetPublicKeyToken());
                requestedAssembly.CultureInfo = CultureInfo.InvariantCulture;

                AppDomain.CurrentDomain.AssemblyResolve -= ResolveAssemblyHandler;
                return Assembly.Load(requestedAssembly);
            }

            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssemblyHandler;
        }

    }
}
