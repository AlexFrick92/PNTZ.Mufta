using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Promatis.Core.Extensions;
using Promatis.Core.Resources;

namespace Promatis.Core.Results
{
    /// <summary>
    /// Результат выполнения операции сохранения. Содержит в себе результат валидации 
    /// </summary>
    [DataContract]
    public class SavingResult : OperationResult, ISavingResult
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="SavingResult"/>
        /// </summary>
        /// <param name="isSuccess">Признак успешного выполнения</param>
        /// <param name="message">Текст сообщения</param>
        public SavingResult(bool isSuccess = false, string message = null)
            : base(isSuccess, message) => ValidationErrors = new Dictionary<string, string>();

        /// <inheritdoc />
        [DataMember]
        public Guid UId { get; set; }

        /// <inheritdoc />
        [DataMember]
        public Dictionary<string, string> ValidationErrors { get; set; }

        /// <inheritdoc />
        /// <exception cref="ArgumentException">Вызывает исключение если <paramref name="message"/> пустое</exception>
        public void AddValidationError(string message, string propertyName = "")
        {
            if(message.IsEmpty())
                throw new ArgumentException(Localization.YouMustDefineErrorMessage, nameof(message));

            var trimmedMessage = message.Trim();
            var trimmedPropertyName = propertyName.Trim();

            if (ValidationErrors.ContainsKey(trimmedPropertyName))
                ValidationErrors[trimmedPropertyName] = $"{ValidationErrors[trimmedPropertyName]}; {trimmedMessage}";
            else
                ValidationErrors.Add(trimmedPropertyName, trimmedMessage);

            Message = string.Empty;
            IsSuccess = false;
        }

        /// <inheritdoc />
        public void AddValidationError(ValidationError error) => AddValidationError(error?.Message, error?.PropertyName);
    }
}