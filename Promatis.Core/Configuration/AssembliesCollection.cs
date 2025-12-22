using Promatis.Core.Extensions;
using Promatis.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Promatis.Core.Configuration
{
    /// <summary>
    /// Коллекция элементов конфигурации, содержащих имена сборок
    /// </summary>
    public sealed class AssembliesCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Источник файлов в виде шаблона имен необходимых сборок. Если не указан путь, то используется рабочий каталог
        /// </summary>
        [ConfigurationProperty("source", IsRequired = false)]
        public string Source
        {
            get => (string)this["source"];
            set => this["source"] = value;
        }

        /// <summary>
        /// Создает новый элемент коллекции
        /// </summary>
        /// <returns>Экземпляр <see cref="AssemblyElement"/></returns>
        protected override ConfigurationElement CreateNewElement() => new AssemblyElement();
        
        /// <summary>
        /// Получает полный путь сборки для элемента коллекции
        /// </summary>
        /// <param name="element">Элемент коллекции <see cref="AssemblyElement"/></param>
        /// <returns>Объект, содержащий имя сборки</returns>
        protected override object GetElementKey(ConfigurationElement element) => ((AssemblyElement)element).Name;

        /// <inheritdoc />
        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap; 

        /// <inheritdoc />
        protected override string ElementName => "assembly";

        /// <summary>
        /// Возвращает список полных путей ко всем сборкам коллекции 
        /// </summary>
        /// <param name="sourcePath">Путь к корневому каталогу</param>
        /// <returns></returns>
        public IEnumerable<string> GetAssembliesPaths(string sourcePath = null)
        {
            var currentPath = Environment.CurrentDirectory;
            if (sourcePath.IsNotEmpty())
            {
                if (sourcePath.Last() != Path.DirectorySeparatorChar)
                    sourcePath += Path.DirectorySeparatorChar;
                Environment.CurrentDirectory = Path.GetFullPath(sourcePath);
            }

            var assemblyPath = Source.IsEmpty() ? Environment.CurrentDirectory : Path.GetFullPath(Path.GetDirectoryName(Source).Or(Environment.CurrentDirectory));
            var assemblyFiles = Directory.EnumerateFiles(Environment.CurrentDirectory, Path.GetFileName(Source), SearchOption.TopDirectoryOnly).ToList();

            BaseGetAllKeys().Cast<string>().ForEach(key =>
            {
                var element = (AssemblyElement)BaseGet(key);
                var elementPath = Path.Combine(Path.GetDirectoryName(element.Name).Or(Environment.CurrentDirectory), Path.GetFileName(element.Name));
                assemblyFiles.Add(Path.GetFullPath(elementPath));
            });
            Environment.CurrentDirectory = currentPath;
            return assemblyFiles.Distinct().ToArray();
        }

        /// <summary>
        /// Загружает в текущий домен и возвращает все сборки коллекции 
        /// </summary>
        public IEnumerable<Assembly> GetAssemblies(string sourcePath = null)
        {
            var assemblyFiles = GetAssembliesPaths(sourcePath);
            return AssemblyHelper.LoadAssemblies(assemblyFiles);
        }

        /// <summary>
        /// Добавляет новый элемент в коллекцию
        /// </summary>
        /// <param name="element">Элемент коллекции</param>
        public void Add(AssemblyElement element) => BaseAdd(element);
    }

    /// <summary>
    /// Элемент коллекции <see cref="AssembliesCollection"/> 
    /// </summary>
    public sealed class AssemblyElement : ConfigurationElement
    {
        /// <summary>
        /// Наименование сборки
        /// </summary>
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get => (string)this["name"];
            set => this["name"] = value;
        }
    }
}
