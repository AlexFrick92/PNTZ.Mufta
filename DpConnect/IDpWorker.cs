

namespace DpConnect
{
    /// <summary>
    /// Классы, в которых создаются свойства с типами IDpValue или IDpAction, должны реализовать этот интерфейс.    
    /// </summary>
    public interface IDpWorker
    {
        /// <summary>
        /// После того, как свойства были привязаны, вызывается этот метод.
        /// </summary>
        void DpBound();
    }
}
