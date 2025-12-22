using System;

namespace Promatis.Core.Attributes
{
    /// <summary>
    /// Задает уникальный идентификатор для поля или типа
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class IdentityAttribute : Attribute
    {
        private string _id;

        /// <summary>
        /// Строковое значение идентификатора
        /// </summary>
        public string ID
        {
            get => _id;
            private set
            {
                Guid uid;
                try
                {
                    uid = new Guid(value);
                }
                catch (Exception)
                {
                    uid = Guid.Empty;
                }

                _id = uid.ToString("B");
            }
        }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="IdentityAttribute"/>
        /// </summary>
        /// <param name="id">Значение идентификатора</param>
        public IdentityAttribute(string id)
        {
            ID = id.Replace("{", "").Replace("}", "").Trim();
        }
    }
}
