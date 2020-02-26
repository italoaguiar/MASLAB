using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MASLAB.Models
{
    public class CommandAdapter : ICommand
    {
        public CommandAdapter()
        {

        }

        public CommandAdapter(bool canExecute)
        {
            SetCanExecute(canExecute);
        }

        public CommandAdapter(Action<object> action)
        {
            Action = action;
        }

        public CommandAdapter(bool canExecute, Action<object> action) : this(canExecute)
        {
            Action = action;
        }

        public event EventHandler CanExecuteChanged;

        bool _canExecute = true;

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public Action<object> Action { get; set; }

        public void SetCanExecute(bool canExecute)
        {
            _canExecute = canExecute;
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public void Execute(object parameter)
        {
            Action?.Invoke(parameter);
        }
    }
}
