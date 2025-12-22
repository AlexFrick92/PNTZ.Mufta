using System;

namespace Promatis.Core.Attributes
{
    /// <summary>
    /// Атрибут, описывающий константу
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class ConstDescriptionAttribute : Attribute
    {
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Короткое наименование
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Код
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ConstDescriptionAttribute"/>
        /// </summary>
        /// <param name="code"></param>
        /// <param name="shortName"></param>
        /// <param name="name"></param>
        public ConstDescriptionAttribute(int code, string shortName, string name)
        {
            Code = code;
            ShortName = shortName;
            Name = name;
        }
    }
}
