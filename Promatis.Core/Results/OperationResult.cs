using System.Runtime.Serialization;
using Promatis.Core.Resources;

namespace Promatis.Core.Results
{
    /// <summary>
    /// Результат выполнения операции
    /// </summary>
    [DataContract]
    public class OperationResult : IOperationResult
    {
        /// <summary>
        /// Текст сообщения, если требуется
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Признак успешного выполнения
        /// </summary>
        [DataMember]
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="isSuccess">Признак успешного выполнения</param>
        /// <param name="message">Текст сообщения</param>
        public OperationResult(bool isSuccess = false, string message = null)
        {
            IsSuccess = isSuccess;
            Message = string.IsNullOrEmpty(message)
                ? (isSuccess ? Localization.OperationResult_SuccessMessage : Localization.OperationResult_FailMessage)
                : message;
        }

        /// <summary>
        /// Оператор неявного преобразования в тип Boolean 
        /// </summary>
        /// <param name="result">Экземпляр для преобразования</param>
        /// <returns>Возвращает флаг успешности выполнения операции</returns>
        public static implicit operator bool(OperationResult result) => result.IsSuccess;

        /// <summary>
        /// Оператор явного преобразования в тип String
        /// </summary>
        /// <param name="result">Экземпляр для преобразования</param>
        /// <returns>Возвращает текст сообщения</returns>
        public static explicit operator string(OperationResult result) => result.Message;

        /// <summary>
        /// Возвращает текст сообщения
        /// </summary>
        /// <returns>Возвращает текст сообщения</returns>
        public override string ToString() => Message;

        /// <summary>
        /// Успешный результат
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static OperationResult Success(string message = null) => new OperationResult(true, message);

        /// <summary>
        /// Провальный результат
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static OperationResult Fail(string message = null) => new OperationResult(false, message);
    }

    /// <summary>
    /// Результат выполнения операции
    /// </summary>
    [DataContract]
    public class OperationResult<T> : OperationResult
    {
        /// <summary>
        /// Возвращаемое значение
        /// </summary>
        [DataMember]
        public T Value { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="isSuccess">Признак успешного выполнения</param>
        /// <param name="message">Текст сообщения</param>
        /// <param name="value">Возвращаемое значение</param>
        public OperationResult(bool isSuccess = false, string message = null, T value = default(T))
            : base(isSuccess, message)
        {
            Value = value;
        }

        /// <summary>
        /// Успешный результат
        /// </summary>
        /// <param name="message"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static OperationResult<T> Success(string message = null, T value = default(T)) => new OperationResult<T>(true, message, value);

        /// <summary>
        /// Провальный результат
        /// </summary>
        /// <param name="message"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static OperationResult<T> Fail(string message = null, T value = default(T)) => new OperationResult<T>(false, message, value);
    }
}