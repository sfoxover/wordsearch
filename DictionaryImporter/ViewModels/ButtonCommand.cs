using System;
using System.Windows.Input;

namespace DictionaryImporter.ViewModels
{
    public class ButtonCommand : ICommand
    {
        // flag to enable execution
        public bool CanExecuteSetting { get; set; }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        // delegate for command click
        public event EventHandler ExecuteDelegate;

        public ButtonCommand(EventHandler handler)
        {
            CanExecuteSetting = true;
            ExecuteDelegate = handler;
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteSetting;
        }

        public void Execute(object parameter)
        {
            ExecuteDelegate?.Invoke(parameter, null);
        }
    }
}

