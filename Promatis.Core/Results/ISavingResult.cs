using System;
using System.Collections.Generic;

namespace Promatis.Core.Results
{
    /// <summary>
    /// Интерфейс типа, описывающего результат выполнения операции сохранения
    /// </summary>
    /// <remarks>Отличается от <see cref="IOperationResult"/> наличием коллекции ошибок валидации</remarks>
    public interface ISavingResult : IOperationResult
    {
        /// <summary>
        /// Идентификатор сохраненного объекта
        /// </summary>
        Guid UId { get; set; }

        /// <summary>
        /// Перечень ошибок валидации объекта. Key - имя свойства, Value - сообщение
        /// </summary>
        Dictionary<string, string> ValidationErrors { get; set; }

        /// <summary>
        /// Добавляет ошибку валидации
        /// </summary>
        /// <param name="message">Текст ошибки</param>
        /// <param name="propertyName">Наименование свойства</param>
        void AddValidationError(string message, string propertyName = "");

        /// <summary>
        /// Добавляет ошибку валидации
        /// </summary>
        /// <param name="error">Ошибка валидации</param>
        void AddValidationError(ValidationError error);

    }
}