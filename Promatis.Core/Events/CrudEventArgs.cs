namespace Promatis.Core.Events
{
    /// <summary>
    /// Аргументы события манипулирования данными (создания, изменения и удаления объектов)
    /// </summary>
    /// <typeparam name="TEntity">Тип данных</typeparam>
    public class CrudEventArgs<TEntity> : EventArgs<TEntity> where TEntity : class
    {
        /// <summary>
        /// Тип изменения
        /// </summary>
        public ChangeType ChangeType { get; }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CrudEventArgs{T}"/>
        /// </summary>
        /// <param name="entity">Данные</param>
        /// <param name="changeType">Тип изменения</param>
        public CrudEventArgs(TEntity entity, ChangeType changeType)
            : base(entity) => ChangeType = changeType;
    }
}
