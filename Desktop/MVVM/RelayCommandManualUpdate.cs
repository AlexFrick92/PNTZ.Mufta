using System;
using System.Windows.Input;

namespace Desktop.MVVM
{
    public class RelayCommandManualUpdate : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        private EventHandler _canExecuteChanged;
        public event EventHandler CanExecuteChanged
        {
            add { _canExecuteChanged += value; }
            remove { _canExecuteChanged -= value; }
        }

        public void RaiseCanExecuteChanged()
        {
            _canExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public RelayCommandManualUpdate(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}
