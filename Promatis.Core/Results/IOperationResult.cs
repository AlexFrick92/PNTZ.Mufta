namespace Promatis.Core.Results
{
    /// <summary>
    /// Интерфейс типа, описывающего результат выполнения операции
    /// </summary>
    public interface IOperationResult
    {
        /// <summary>
        /// Текст сообщения, если требуется
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// Признак успешного выполнения
        /// </summary>
        bool IsSuccess { get; set; }
    }
}