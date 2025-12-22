namespace Promatis.Core
{
    /// <summary>
    /// Интерфейс конвертера объекта к типу-представлению
    /// </summary>
    /// <typeparam name="TEntity">Объект</typeparam>
    /// <typeparam name="TView">Тип представления</typeparam>
    public interface IConverter<in TEntity, out TView>
    {
        /// <summary>
        /// Преобразует объект в тип - представление
        /// </summary>
        /// <param name="entity">Объект</param>
        /// <returns>Тип - представление</returns>
        TView Convert(TEntity entity);
    }
}