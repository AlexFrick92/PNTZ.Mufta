namespace Promatis.Core
{
	/// <summary>
	/// Тип действия, произведенного над объектом
	/// </summary>
	public enum ChangeType
	{
	    /// <summary>
	    /// Без изменений
	    /// </summary>
	    None = 0,

        /// <summary>
        /// Создание
        /// </summary>
		Create = 1,

        /// <summary>
        /// Редактирование
        /// </summary>
		Update = 2,

        /// <summary>
        /// Удаление
        /// </summary>
		Delete = 3,
	}
}
