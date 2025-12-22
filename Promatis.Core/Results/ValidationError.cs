using System;
using System.Runtime.Serialization;
using Promatis.Core.Resources;

namespace Promatis.Core.Results
{
    /// <summary>
    /// Ошибка валидации
    /// </summary>
    [DataContract]
    public class ValidationError
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ValidationError"/>
        /// </summary>
        /// <param name="message">Текст ошибки</param>
        /// <param name="propertyName">Наименование свойства</param>
        public ValidationError(string message, string propertyName = "")
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException(Localization.YouMustDefineErrorMessage, nameof(message));

            Message = message;
            PropertyName = propertyName;
        }

        /// <summary>
        /// Текст ошибки
        /// </summary>
        [DataMember]
        public string Message { get; private set; }

        /// <summary>
        /// Наименование свойства
        /// </summary>
        [DataMember]
        public string PropertyName { get; private set; }
    }
}