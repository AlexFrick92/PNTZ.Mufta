using System.Linq;
using System.Xml.Linq;

namespace Promatis.Core.Extensions
{
    /// <summary>
    /// Методы расштрения для xml контейнера
    /// </summary>
    public static class XContainerExtensions
    {
        /// <summary>
        /// Получает элемент из контайнера по имени. Если элемента нет, то создает его с указанными атрубутами
        /// </summary>
        /// <param name="container">Контейнер.</param>
        /// <param name="elementName">Наименование элемента.</param>
        /// <param name="attributes">Коллекция аттрибутов.</param>
        /// <returns></returns>
        public static XElement GetOrCreateElement(
            this XContainer container, string elementName, params object[] attributes)
        {
            Guard.IsNotNull(container);
            Guard.IsNotEmpty(elementName);

            var element = container.Element(elementName);
            if (element == null)
            {
                element = new XElement(elementName, attributes);
                container.Add(element);
            }
            return element;
        }

        /// <summary>
        /// Создает или обновляет текущий элемент в контейнере.
        /// </summary>
        /// <param name="container">Контейнер.</param>
        /// <param name="elementName">Наименование элемента.</param>
        /// <param name="attributes">Коллекция аттрибутов элемента.</param>
        /// <returns></returns>
        public static XElement CreateOrUpdateElement(
            this XContainer container, string elementName, params object[] attributes)
        {
            var element = GetOrCreateElement(container, elementName, attributes);
            if (!element.Attributes().SequenceEqual(attributes))
            {
                var xElement = container.Element(elementName);
                xElement?.Remove();
                element = new XElement(elementName, attributes);
                container.Add(element);
            }
            return element;
        }

        /// <summary>
        /// Проверяет наличие в контейнере элемента с указанным наименованием.
        /// </summary>
        /// <param name="container">Контейнер.</param>
        /// <param name="name">Наименование элемента.</param>
        /// <returns>True если элемент присутствует, иначе - False</returns>
        public static bool HasElement(this XContainer container, string name) => container.Element(name) != null;
    }

}
