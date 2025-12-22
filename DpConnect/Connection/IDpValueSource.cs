using System;


namespace DpConnect.Connection
{
    public interface IDpValueSource<T> : IDpStatus
    {
        void UpdateValueFromSource(T value);

        event EventHandler<T> ValueWritten;
    }
}
