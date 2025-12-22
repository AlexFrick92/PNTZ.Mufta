namespace Promatis.Core
{
    /// <summary>
    /// Интерфейс объекта, имеющего внешний идентификатор
    /// </summary>
    public interface IHaveExternalId
    {
        /// <summary>
        /// Внешний идентификатор
        /// </summary>
        int? ExternalId { get; set; }
    }
}
