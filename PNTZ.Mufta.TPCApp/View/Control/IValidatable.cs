namespace PNTZ.Mufta.TPCApp.View.Control
{
    /// <summary>
    /// Интерфейс для контролов с валидацией
    /// </summary>
    public interface IValidatable
    {
        /// <summary>
        /// Флаг наличия ошибки валидации
        /// </summary>
        bool IsValidationError { get; }
    }
}
