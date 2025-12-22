namespace Promatis.Core.Interfaces
{
    /// <summary>
    /// Интерфейс объекта, идентифицируемого по целочисленному идентификатору
    /// </summary>
    public interface IIdentifiedByInt
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        int Id { get; }
    }
}
