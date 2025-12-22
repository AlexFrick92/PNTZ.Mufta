using System;
using System.Timers;
using SysTimer = System.Timers.Timer;

namespace Toolkit.Reactive
{
    public class EventDebounce<T>
    {

        private SysTimer _throttleTimer;
        public EventDebounce(int ThrottleFilter = 0)
        {
            if(ThrottleFilter > 0)
            {
                _throttleTimer = new SysTimer(ThrottleFilter);
                _throttleTimer.AutoReset = false;
                _throttleTimer.Elapsed += OnDebounceElapsed;
            }
        }

        private void OnDebounceElapsed(object sender, ElapsedEventArgs e)
        {
            _timerStarted = false;
            DebouncedEvent?.Invoke(_sender, _eventValue);
        }

        bool _timerStarted;

        object _sender;
        T _eventValue;

        public event EventHandler<T> DebouncedEvent;

        public void RiseEvent(object sender, T arg)
        {
            _sender = sender;
            _eventValue = arg;

            if(_throttleTimer != null)
            {
                if(!_timerStarted)
                {
                    _throttleTimer.Start();
                    _timerStarted = true;
                }
            }
            else
            {
                DebouncedEvent?.Invoke(sender, arg);
            }
        }



    }
}
