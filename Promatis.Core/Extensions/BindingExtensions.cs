using System.ServiceModel;

namespace Promatis.Core.Extensions
{
    /// <summary>
    /// Методы расширения для привязок
    /// </summary>
    public static class BindingExtensions
    {
        /// <summary>
        /// Задает размер буфера данных
        /// </summary>
        /// <remarks>Изменяет значения свойств MaxBufferPoolSize, MaxBufferSize, MaxReceivedMessageSize</remarks>
        /// <param name="binding">Привязка</param>
        /// <param name="bufferSize">Размер буфера (в байтах)</param>
        /// <returns></returns>
        public static NetTcpBinding SetBufferSize(this NetTcpBinding binding, int bufferSize)
        {
            binding.MaxBufferPoolSize = bufferSize;
            binding.MaxBufferSize = bufferSize;
            binding.MaxReceivedMessageSize = bufferSize;
            return binding;
        }
    }
}
