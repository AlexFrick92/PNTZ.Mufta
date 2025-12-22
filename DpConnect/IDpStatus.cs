using System;

namespace DpConnect
{
    /// <summary>
    /// Статус точки. Запись в точку или вызов метода вызовет исключение, если соединение не активно.
    /// </summary>
    public interface IDpStatus
    {
        bool IsConnected { get; set; }

        event EventHandler<EventArgs> StatusChanged;
    }
}
