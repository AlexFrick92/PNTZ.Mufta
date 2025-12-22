using System;

namespace DpConnect
{
    /// <summary>
    /// Объявляется как открытое свойство IDpWorker. В это свойством будет присвоен объект, через который поступают данные
    /// </summary>
    /// <typeparam name="T"> Тип Т - примитивный тип, DateTime, string или класс с открытыми свойствами. Имя свойства должно совпадать с именем свойства в структуре OPC UA </typeparam>
    public interface IDpValue<T> : IDpStatus 
    {
        /// <summary>
        /// Свойство, через которое можно получить актуальное значение
        /// Через этой свойство так же записывается значение
        /// </summary>
        T Value { get; set; }

        /// <summary>
        /// Возникает при обновлении Value из источника данных
        /// </summary>
        event EventHandler<T> ValueUpdated;        
    }
}
