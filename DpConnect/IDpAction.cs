using System;

namespace DpConnect
{
    /// <summary>
    /// Объявляется как открытое свойство IDpWorker. В это свйоство будет присвоен объект, через который вызывается метод
    /// </summary>
    /// <typeparam name="T">Делегат, который может вернуть либо простой тип, DateTime или string, либо объект класса, в котором объявлены открытые свойства. Свойства будут заполнены в порядке их объявления</typeparam>
    public interface IDpAction<T> : IDpStatus where T : Delegate
    {
        /// <summary>
        /// Вызов метода на сервере
        /// </summary>
        T Call { get; }        
    }
}
